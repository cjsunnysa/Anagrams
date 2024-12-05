using Anagrams.Services;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.Collections.Immutable;

namespace Anagrams;

[SimpleJob(RunStrategy.ColdStart, iterationCount: 1)]
[MemoryDiagnoser]
public class AnagramBenchmarks
{
    private ImmutableArray<string> _words;
    private ImmutableArray<string> _filteredWords;

    [ParamsSource(nameof(Data))]
    public int[] WordsLength { get; set; } = [];

    public object[] Data()
    {
        return [new int[] { 2, 3, 4, 5 }];
    }

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
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _filteredWords = _words.Where(x => WordsLength.Contains(x.Length)).ToImmutableArray();
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
