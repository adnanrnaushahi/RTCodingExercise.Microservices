using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
namespace Catalog.API.Services
{
    public class PlateService : IPlateService
    {
        public Task<IEnumerable<Plate>> GetAllPlatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Plate> GetPlateByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
