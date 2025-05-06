using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
namespace Catalog.API.Services
{
    public class PlateService : IPlateService
    {
        private readonly IPlateRepository _plateRepository;
        private readonly IPlateDetailRepository _plateDetailRepository;
        private readonly ILogger<PlateService> _logger;

        public PlateService(IPlateRepository plateRepository, IPlateDetailRepository plateDetailRepository, ILogger<PlateService> logger)
        {
            _plateRepository = plateRepository;
            _plateDetailRepository = plateDetailRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Plate>> GetAllPlatesAsync()
        {
            return await _plateRepository.GetAllPlatesAsync();
        }

        public async Task<Plate?> GetPlateByIdAsync(Guid id)
        {
            return await _plateRepository.GetPlateByIdAsync(id);
        }
    }
}
