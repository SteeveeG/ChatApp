using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ChatApp.ApiHandler;

public class ApiGet
{
    public static async Task<T> GetApiIn<T>(string requestUri)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7049");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync(requestUri);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        return default;
    }
}
public class ApiPost
{
    public static async Task<T> Post<T>(string requestUri , HttpContent content)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7049");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.PostAsync(requestUri, content);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
        return default;
    } 
}

public class ApiDelete
{
    public static async Task<T> Delete<T>(string requestUri)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7049");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.DeleteAsync(requestUri);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
        return default;
    }
    
}