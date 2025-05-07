using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces
{
    public interface IPlateRepository
    {
        Task<IEnumerable<Plate>> GetPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true);
        Task<Plate> AddPlateAsync(Plate plate);
        Task UpdatePlateAsync(Plate plate);
        Task DeletePlateAsync(Guid id);
        Task<Plate?> GetPlateByIdAsync(Guid id);
        Task<int> GetTotalPlatesCountAsync();
    }
}
