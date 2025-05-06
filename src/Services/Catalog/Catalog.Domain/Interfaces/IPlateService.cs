
using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces
{
    public interface IPlateService
    {
        Task<IEnumerable<Plate>> GetAllPlatesAsync();
        Task<Plate> GetPlateByIdAsync(Guid id);
    }
}
