using Anagrams;
using Anagrams.Services;
using BenchmarkDotNet.Running;
using System.Collections.Immutable;

try
{
    var summary = BenchmarkRunner.Run<AnagramBenchmarks>();

    //var httpClient = new HttpClient
    //{
    //    BaseAddress = new Uri("https://raw.githubusercontent.com/"),
    //    Timeout = TimeSpan.FromSeconds(30)
    //};

    //var httpService = new HttpService(httpClient);

    //var words = await httpService.FetchWords();

    //var anagramAlgorithm = new LetterCountAnagramCompareAlgorithm();

    //var anagramCalculator = new MultithreadedLengthPartitionedAnagramCalculator(anagramAlgorithm, words);

    //var anagramGroups = anagramCalculator.GetAllAnagrams();

    //foreach (var group in anagramGroups)
    //{
    //    Console.WriteLine(group);
    //}
}
catch (Exception ex)
{
    Console.WriteLine($"Stuff exploded - {ex.Message}");
}