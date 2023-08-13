using System.Net.Http.Headers;
using System.Text.Json;

namespace WorkoutBuddy.Authentication;

public static class HttpExtensions
{
    public static async Task<R?> PostAsJsonAsync<T, R>(
        this HttpClient httpClient, string url, T data)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var dataAsString = JsonSerializer.Serialize(data, options);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var repsonseAsString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<R>(repsonseAsString);
        }
        else
        {
            return default;
        }


    }
}
