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

    //var wordsSubset = words.Where(x => x.Length == 6).OrderBy(x => x.Length).ThenBy(x => x).ToImmutableArray();

    //var anagramAlgorithm = new SortCompareAnagramAlgorithm();

    //var anagramCalculator = new AnagramCalculator(anagramAlgorithm, wordsSubset);

    //var anagramGroups = anagramCalculator.GetHighestScoredAnagrams(take: 5);

    //foreach (var group in anagramGroups)
    //{
    //    Console.WriteLine(group);
    //}
}
catch (Exception ex)
{
    Console.WriteLine($"Stuff exploded - {ex.Message}");
}