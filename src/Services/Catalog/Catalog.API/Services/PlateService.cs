using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
namespace Catalog.API.Services
{
    public class PlateService : IPlateService
    {
        private readonly IPlateRepository _plateRepository;

        public PlateService(IPlateRepository plateRepository)
        {
            _plateRepository = plateRepository;
        }

        public Task<Plate> CreatePlateAsync(string registration, decimal purchasePrice, decimal salePrice, string letters, int numbers)
        {
            throw new NotImplementedException();
        }

        public async Task<Plate?> GetPlateByIdAsync(Guid id)
        {
            return await _plateRepository.GetPlateByIdAsync(id);
        }

        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesAsync(int pageSize, int pageIndex)
        {
            var plates = await _plateRepository.GetPlatesAsync(pageSize, pageIndex);
            var totalCount = await _plateRepository.GetTotalPlatesCountAsync();
            return (plates, totalCount);
        }
    }
}
