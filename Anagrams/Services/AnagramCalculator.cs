using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Text;

namespace Anagrams.Services;

internal sealed class AnagramGroup(IEnumerable<string> words, int points)
{
    public ImmutableArray<string> Words { get; } = [.. words];
    public int Points { get; } = points;

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var word in Words)
        {
            sb.Append($", {word}");
        }

        sb.Remove(0, 2);

        return $"Words: [{sb}], Points: {Points}";
    }
}

internal interface IAnagramCompareAlgorithm
{
    bool IsAnagram(string s1, string s2);
}

internal sealed class LetterCountAnagramCompareAlgorithm : IAnagramCompareAlgorithm
{
    public bool IsAnagram(string s1, string s2)
    {
        if (s1.Length != s2.Length)
        {
            return false;
        }

        var chars = new Dictionary<char, int>(s1.Length);

        foreach (char ch in s1)
        {
            chars[ch] = chars.GetValueOrDefault(ch, 0) + 1;
        }

        foreach (char ch in s2)
        {
            if (!chars.TryGetValue(ch, out var charCount) || charCount == 0)
            {
                return false;
            }

            chars[ch] -= 1;
        }

        return true;
    }
}

internal sealed class SortAnagramCompareAlgorithm : IAnagramCompareAlgorithm
{
    public bool IsAnagram(string s1, string s2)
    {
        if (s1.Length != s2.Length || s1 == s2)
        {
            return false;
        }

        var sorted1 = s1.Order();
        var sorted2 = s2.Order();

        return sorted1.SequenceEqual(sorted2);
    }
}

internal sealed class SingleThreadedLengthPartitionedAnagramCalculator(IAnagramCompareAlgorithm anagramAlgorithm, IEnumerable<string> words) : AnagramCalculator(anagramAlgorithm, words)
{
    protected override ImmutableArray<AnagramGroup> CreateAnagramGroups(IAnagramCompareAlgorithm anagramAlgorithm, IEnumerable<string> words)
    {
        return
            words
                .GroupBy(x => x.Length)
                .SelectMany(x => CreateAnagramGroupsForSection(anagramAlgorithm, [.. x]))
                .ToImmutableArray();
    }
}

internal sealed class MultithreadedLengthPartitionedAnagramCalculator(IAnagramCompareAlgorithm anagramAlgorithm, IEnumerable<string> words) : AnagramCalculator(anagramAlgorithm, words)
{
    protected override ImmutableArray<AnagramGroup> CreateAnagramGroups(IAnagramCompareAlgorithm anagramAlgorithm, IEnumerable<string> words)
    {
        return
            words
                .GroupBy(x => x.Length)
                .AsParallel()
                .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                .SelectMany(x => CreateAnagramGroupsForSection(anagramAlgorithm, x.ToImmutableArray()))
                .ToImmutableArray();
    }
}

internal sealed class UnpartitionedAnagramCalculator(IAnagramCompareAlgorithm anagramAlgorithm, IEnumerable<string> words) : AnagramCalculator(anagramAlgorithm, words)
{
    protected override ImmutableArray<AnagramGroup> CreateAnagramGroups(IAnagramCompareAlgorithm anagramAlgorithm, IEnumerable<string> words)
    {
        return CreateAnagramGroupsForSection(anagramAlgorithm, [.. words]);
    }
}

internal abstract class AnagramCalculator
{
    private static class WordScorer
    {
        private sealed record PointRule(IEnumerable<char> Characters, int Point);

        private static readonly PointRule[] _pointRules = [
            new (Characters: ['a', 'e', 'i', 'l', 'n', 'o', 'r', 's', 't', 'u'], Point: 1),
            new (Characters: ['d', 'g'], Point: 2),
            new (Characters: ['b', 'c', 'm', 'p'], Point: 3),
            new (Characters: ['f', 'h', 'v', 'w', 'y'], Point: 4),
            new (Characters: ['k'], Point: 5),
            new (Characters: ['j', 'x'], Point: 8),
            new (Characters: ['q', 'z'], Point: 10)
        ];

        public static int CalculatePoints(string word)
        {
            return word.Aggregate(0, (total, character) =>
                total + _pointRules.First(x => x.Characters.Contains(character)).Point);
        }
    }

    private readonly ImmutableArray<AnagramGroup> _anagrams;

    public AnagramCalculator(IAnagramCompareAlgorithm anagramAlgorithm, IEnumerable<string> words)
    {
        _anagrams = CreateAnagramGroups(anagramAlgorithm, words);
    }

    protected abstract ImmutableArray<AnagramGroup> CreateAnagramGroups(IAnagramCompareAlgorithm anagramAlgorithm, IEnumerable<string> words);

    protected ImmutableArray<AnagramGroup> CreateAnagramGroupsForSection(IAnagramCompareAlgorithm anagramAlgorithm, IList<string> words)
    {
        int foundIndexSize = Convert.ToInt32(words.Count * 0.1);

        HashSet<int> foundIndexes = new HashSet<int>(foundIndexSize);

        var results = new LinkedList<AnagramGroup>();

        for (int i = 0; i < words.Count; i++)
        {
            var candidateWord = words[i];

            var anagrams = new LinkedList<string>();
            
            anagrams.AddLast(candidateWord);

            for (int j = i + 1; j < words.Count; j++)
            {
                var compareWord = words[j];

                if (candidateWord.Length != compareWord.Length || foundIndexes.Contains(j))
                {
                    continue;
                }

                if (anagramAlgorithm.IsAnagram(candidateWord, compareWord))
                {
                    foundIndexes.Add(j);
                    anagrams.AddLast(compareWord);
                }
            }

            if (anagrams.Count > 1)
            {
                var points = WordScorer.CalculatePoints(candidateWord);

                results.AddLast(new AnagramGroup(anagrams, points));
            }
        }

        return [.. results];
    }

    public ImmutableArray<AnagramGroup> GetAllAnagrams()
    {
        return [.. _anagrams.OrderBy(x => x.Points)];
    }

    public ImmutableArray<AnagramGroup> GetHighestScoredAnagrams(int take = 1)
    {
        return
            _anagrams
                .OrderByDescending(x => x.Points)
                .Take(take)
                .ToImmutableArray();
    }
}
