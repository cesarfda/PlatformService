using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpCommandDataClient> _logger;
        private readonly IConfiguration _config;

        public HttpCommandDataClient(HttpClient httpClient, ILogger<HttpCommandDataClient> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config;
        }
        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_config["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("--> Sync POST to Command Service was OK!");
            }
            else
            {
                _logger.LogError("--> Sync POST to Command Service was NOT OK!");
            }
        }
    }
}