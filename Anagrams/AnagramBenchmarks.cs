using Anagrams.Services;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.Collections.Immutable;

namespace Anagrams;

[SimpleJob(RunStrategy.ColdStart, iterationCount: 1)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class AnagramBenchmarks
{
    //private readonly int[] _wordLengths = [ 2, 3, 4, 5, 6, 7 ];

    private ImmutableArray<string> _words;

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

        //_words = words.Where(x => _wordLengths.Contains(x.Length)).ToImmutableArray();
    }

    [Benchmark]
    public void LetterCountAlgorithmUnpartitioned()
    {
        var calculator = new UnpartitionedAnagramCalculator(new LetterCountAnagramCompareAlgorithm(), _words);

        _ = calculator.GetAllAnagrams();
    }

    [Benchmark]
    public void LetterCountAlgorithmSingleThreadedLengthPartitioned()
    {
        var calculator = new SingleThreadedLengthPartitionedAnagramCalculator(new LetterCountAnagramCompareAlgorithm(), _words);

        _ = calculator.GetAllAnagrams();
    }

    [Benchmark]
    public void LetterCountAlgorithmMultithreadedLengthPartitioned()
    {
        var calculator = new MultithreadedLengthPartitionedAnagramCalculator(new LetterCountAnagramCompareAlgorithm(), _words);

        _ = calculator.GetAllAnagrams();
    }
}
