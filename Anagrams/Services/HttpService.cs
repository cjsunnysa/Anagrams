using System.Collections.Immutable;
using System.Net.Http.Json;

namespace Anagrams.Services;

internal sealed class HttpService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<ImmutableArray<string>> FetchWords()
    {
        var responseMessage = await _httpClient.GetAsync("/benjamincrom/scrabble/master/scrabble/dictionary.json");

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new Exception("Http request returned unsuccessful status code.");
        }
            
        var words = await responseMessage.Content.ReadFromJsonAsync<string[]>();

        if (words is null)
        {
            throw new Exception("Http response content is invalid.");
        }

        return [.. words];
    }
}
