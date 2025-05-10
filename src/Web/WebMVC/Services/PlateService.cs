using System.Text;
using System.Text.Json;
using Catalog.Domain.Enum;
using Catalog.Domain.Models;
using MassTransit;
using Newtonsoft.Json;
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

        public async Task<PaginatedItemsViewModel<PlateViewModel>> GetAllPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true, PlateStatus? status = null)
        {

            var statusParam = status.HasValue ? $"&status={status.Value}" : "";
            var uri = new Uri($"{_catalogApiUrl}/api/plates?pageSize={pageSize}&pageIndex={pageIndex}&orderByAsc={orderByAsc}{statusParam}");
            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error getting all plates: {response.StatusCode}, {errorContent}");
            }

            return JsonConvert.DeserializeObject<PaginatedItemsViewModel<PlateViewModel>>(await response.Content?.ReadAsStringAsync());
        }

        public async Task<PlateViewModel> GetPlateByIdAsync(Guid id)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/{id}");
            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error getting plate by Id: {response.StatusCode}, {errorContent}");
            }

            return JsonConvert.DeserializeObject<PlateViewModel>(await response.Content?.ReadAsStringAsync());
        }

        public async Task<PlateViewModel> CreatePlateAsync(CreatePlateViewModel model)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_catalogApiUrl}/api/plates", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error creating plate: {response.StatusCode}, {errorContent}");
            }

            var plateContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PlateViewModel>(await response.Content?.ReadAsStringAsync());           
        }

        public async Task<PaginatedItemsViewModel<PlateViewModel>> FilterByLettersAsync(string letters, int pageSize, int pageIndex)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/filterByLetters?letters={Uri.EscapeDataString(letters)}&pageSize={pageSize}&pageIndex={pageIndex}");
            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error filtering plates by letters: {response.StatusCode}, {errorContent}");
            }

            return JsonConvert.DeserializeObject<PaginatedItemsViewModel<PlateViewModel>>(await response.Content?.ReadAsStringAsync());
        }

        public async Task<PaginatedItemsViewModel<PlateViewModel>> FilterByNumbersAsync(string numbers, int pageSize, int pageIndex)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/filterByNumbers?numbers={Uri.EscapeDataString(numbers)}&pageSize={pageSize}&pageIndex={pageIndex}");
            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error filtering by numbers: {response.StatusCode}, {errorContent}");
            }

            return JsonConvert.DeserializeObject<PaginatedItemsViewModel<PlateViewModel>>(await response.Content?.ReadAsStringAsync());
        }

        public async Task<PaginatedItemsViewModel<PlateViewModel>> SearchPlatesAsync(string query, int pageSize, int pageIndex)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/search?query={Uri.EscapeDataString(query)}&pageSize={pageSize}&pageIndex={pageIndex}");
            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error search plate: {response.StatusCode}, {errorContent}");
            }
            ;

            return JsonConvert.DeserializeObject<PaginatedItemsViewModel<PlateViewModel>>(await response.Content?.ReadAsStringAsync());
        }

        public async Task<PlateViewModel> UpdatePlateStatusAsync(Guid id, PlateStatus newStatus)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(newStatus), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_catalogApiUrl}/api/plates/{id}/status", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error update plate status: {response.StatusCode}, {errorContent}");
            }

            return JsonConvert.DeserializeObject<PlateViewModel>(await response.Content?.ReadAsStringAsync());
        }

        public async Task<RevenueViewModel> GetTotalRevenueAsync()
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates/GetRevenue");
            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error getting revenue: {response.StatusCode}, {errorContent}");
            }

            return JsonConvert.DeserializeObject<RevenueViewModel>(await response.Content?.ReadAsStringAsync());
        }
    }
}
