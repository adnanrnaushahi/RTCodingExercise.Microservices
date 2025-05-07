using WebMVC.Models;
using WebMVC.Utils;

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

        public async Task<PaginatedItemsViewModel<PlateViewModel>> GetAllPlatesAsync(int pageSize, int pageIndex)
        {
            var uri = new Uri($"{_catalogApiUrl}/api/plates?pageSize={pageSize}&pageIndex={pageIndex}");
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
    }
}
