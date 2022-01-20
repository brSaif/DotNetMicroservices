using System.Text.Json;
using PlatformService.Dtos;

namespace  PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public HttpCommandDataClient(HttpClient client, 
                                IConfiguration config)
    {
        _httpClient = client;
        _config = config;
    }

    public async Task SendPlatformsToCommand(PlatformReadDto plat)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(plat),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var _uri = _config["CommandService"];
        var response = await _httpClient.PostAsync(
                _uri,
                httpContent);

        if (response.IsSuccessStatusCode)
            Console.WriteLine("---> sync post is ok");
        else
            Console.WriteLine("---> sync post is not ok :" + _uri);
    }
}

