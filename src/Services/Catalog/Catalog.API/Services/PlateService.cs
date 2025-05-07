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

        public async Task<Plate> CreatePlateAsync(string registration, decimal purchasePrice, decimal salePrice, string letters, int numbers)
        {
            var plate = new Plate(registration, purchasePrice, salePrice, letters, numbers);
            return await _plateRepository.AddPlateAsync(plate);
        }

        public async Task<Plate?> GetPlateByIdAsync(Guid id)
        {
            return await _plateRepository.GetPlateByIdAsync(id);
        }

        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true)
        {
            var plates = await _plateRepository.GetPlatesAsync(pageSize, pageIndex, orderByAsc);
            var totalCount = await _plateRepository.GetTotalPlatesCountAsync();
            return (plates, totalCount);
        }
    }
}
