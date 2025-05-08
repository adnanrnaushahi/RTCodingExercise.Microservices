using System.Text;
using System.Text.Json;
using WebMVC.Utils;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public class PlateService : IPlateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _catalogApiUrl;

        public PlateService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _catalogApiUrl = settings.Value.CatalogApiUrl;
        }

        public async Task<PaginatedItemsViewModel<PlateViewModel>> GetAllPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates?pageSize={pageSize}&pageIndex={pageIndex}&orderByAsc={orderByAsc}");
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConverterUtils.ToPaginatedPlatesViewModel(content);
        }

        public async Task<PlateViewModel> GetPlateByIdAsync(Guid id)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/{id}");
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConverterUtils.ToPlateViewModel(content);
        }

        public async Task<PlateViewModel> CreatePlateAsync(CreatePlateViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_catalogApiUrl}/api/plates", content);
            response.EnsureSuccessStatusCode();

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error creating plate: {response.StatusCode}, {errorContent}");
            }

            var plateContent = await response.Content.ReadAsStringAsync();
            return JsonConverterUtils.ToPlateViewModel(plateContent);
        }

        public async Task<PaginatedItemsViewModel<PlateViewModel>> FilterByLettersAsync(string letters, int pageSize, int pageIndex)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/filterByLetters?letters={Uri.EscapeDataString(letters)}&pageSize={pageSize}&pageIndex={pageIndex}");
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConverterUtils.ToPaginatedPlatesViewModel(content);
        }

        public async Task<PaginatedItemsViewModel<PlateViewModel>> FilterByNumbersAsync(string numbers, int pageSize, int pageIndex)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/filterByNumbers?numbers={Uri.EscapeDataString(numbers)}&pageSize={pageSize}&pageIndex={pageIndex}");
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConverterUtils.ToPaginatedPlatesViewModel(content);
        }

        public async Task<PaginatedItemsViewModel<PlateViewModel>> SearchPlatesAsync(string query, int pageSize, int pageIndex)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/search?query={Uri.EscapeDataString(query)}&pageSize={pageSize}&pageIndex={pageIndex}");
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConverterUtils.ToPaginatedPlatesViewModel(content);
        }
    }
}
