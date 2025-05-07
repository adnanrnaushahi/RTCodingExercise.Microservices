using System.Text.Json;
using System.Text.Json.Serialization;
using WebMVC.Models;

namespace WebMVC.Services
{
    public class PlateService : IPlateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlateService> _logger;
        private readonly string _catalogApiUrl;

        public PlateService(HttpClient httpClient, IOptions<AppSettings> settings, ILogger<PlateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _catalogApiUrl = settings.Value.CatalogApiUrl;
        }

        public async Task<IEnumerable<PlateViewModel>> GetAllPlatesAsync()
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates");
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var plates = JsonSerializer.Deserialize<IEnumerable<PlateViewModel>>(content,
             new JsonSerializerOptions
             {
                 PropertyNameCaseInsensitive = true,
                 Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
             });

            return plates;
        }

        public async Task<PlateViewModel> GetPlateByIdAsync(Guid id)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/{id}");
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var plate = JsonSerializer.Deserialize<PlateViewModel>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return plate;
        }
    }
}
