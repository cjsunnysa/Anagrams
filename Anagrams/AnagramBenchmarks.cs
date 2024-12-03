using Anagrams.Services;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.Collections.Immutable;

namespace Anagrams;

[SimpleJob(RunStrategy.ColdStart, iterationCount: 2)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class AnagramBenchmarks
{
    private readonly int[] _wordLengths = [2, 3, 4, 5, 6, 7, 8];

    private ImmutableArray<string> _words;

    private ImmutableArray<string> _filteredWords = [];

    [GlobalSetup]
    public async Task Setup()
    {
        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://raw.githubusercontent.com/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        var httpService = new HttpService(httpClient);

        _words = await httpService.FetchWords();

        _filteredWords =
            _words
                .Where(x => _wordLengths.Contains(x.Length))
                .OrderBy(x => x.Length)
                .ThenBy(x => x)
                .ToImmutableArray();
    }

    [Benchmark]
    public void GetAnagramsUsingDictionaryCountAlgorithmSingleThread()
    {
        var calculator = new AnagramCalculator(new DictionaryCountAnagramAlgorithm(), _filteredWords);

        _ = calculator.GetAllAnagrams();
    }

    [Benchmark]
    public void GetAnagramsUsingDictionaryCountAlgorithmInParallel()
    {
        var calculator = new ParallelAnagramCalculator(new DictionaryCountAnagramAlgorithm(), _filteredWords);

        _ = calculator.GetAllAnagrams();
    }
}
