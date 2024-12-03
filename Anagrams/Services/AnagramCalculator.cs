using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;

namespace Anagrams.Services;

internal sealed class AnagramGroup(List<string> words, int points)
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

internal interface IAnagramAlgorithm
{
    bool IsAnagram(string s1, string s2);
}

internal sealed class DictionaryCountAnagramAlgorithm : IAnagramAlgorithm
{
    public bool IsAnagram(string s1, string s2)
    {
        if (s1.Length != s2.Length)
        {
            return false;
        }

        var chars = new Dictionary<char, int>();

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

internal sealed class SortCompareAnagramAlgorithm : IAnagramAlgorithm
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

internal class AnagramCalculator
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

    public AnagramCalculator(IAnagramAlgorithm anagramAlgorithm, ImmutableArray<string> words)
    {
        _anagrams = CreateAnagramGroups(anagramAlgorithm, words);
    }

    protected virtual ImmutableArray<AnagramGroup> CreateAnagramGroups(IAnagramAlgorithm anagramAlgorithm, ImmutableArray<string> words)
    {
        return CreateAnagramGroupsForSection(anagramAlgorithm, words);
    }

    protected ImmutableArray<AnagramGroup> CreateAnagramGroupsForSection(IAnagramAlgorithm anagramAlgorithm, ImmutableArray<string> words)
    {
        HashSet<int> anagramIndexes = [];

        List<AnagramGroup> results = [];

        for (int i = 0; i < words.Length; i++)
        {
            var candidateWord = words[i];

            var anagrams = new List<string> { candidateWord };

            for (int j = i + 1; j < words.Length; j++)
            {
                if (anagramIndexes.Contains(j))
                {
                    continue;
                }

                var compareWord = words[j];

                if (compareWord.Length > candidateWord.Length)
                {
                    break;
                }

                if (anagramAlgorithm.IsAnagram(candidateWord, compareWord))
                {
                    anagrams.Add(compareWord);
                    anagramIndexes.Add(j);
                }
            }

            if (anagrams.Count > 1)
            {
                var points = WordScorer.CalculatePoints(candidateWord);

                results.Add(new AnagramGroup(anagrams, points));
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

internal class ParallelAnagramCalculator : AnagramCalculator
{
    public ParallelAnagramCalculator(IAnagramAlgorithm anagramAlgorithm, ImmutableArray<string> words) 
        : base(anagramAlgorithm, words)
    { }

    protected override ImmutableArray<AnagramGroup> CreateAnagramGroups(IAnagramAlgorithm anagramAlgorithm, ImmutableArray<string> words)
    {
        var parallelQuery =
            words
                .GroupBy(x => x.Length)
                .AsParallel()
                .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                .Select(x => CreateAnagramGroupsForSection(anagramAlgorithm, [.. x]))
                .SelectMany(x => x);

        var response = new ConcurrentBag<AnagramGroup>();

        foreach (var group in parallelQuery)
        {
            response.Add(group);
        }

        return [.. response];
    }
}
