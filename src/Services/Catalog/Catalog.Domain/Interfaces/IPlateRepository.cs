using Catalog.Domain.Entities;
using Catalog.Domain.Enum;
using Catalog.Domain.Models;

namespace Catalog.Domain.Interfaces
{
    public interface IPlateRepository
    {
        Task<IEnumerable<Plate>> GetPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true, PlateStatus? status = null);
        Task<Plate> AddPlateAsync(Plate plate);
        Task UpdatePlateAsync(Plate plate);
        Task<Plate?> GetPlateByIdAsync(Guid id);
        Task<int> GetTotalPlatesCountAsync(PlateStatus? status = null);
        Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesByLettersAsync(string letters, int pageSize, int pageIndex);
        Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesByNumbersAsync(string numbers, int pageSize, int pageIndex);
        Task<(IEnumerable<Plate> Plates, int TotalCount)> SearchPlatesAsync(string query, int pageSize, int pageIndex);
        Task<Revenue> GetTotalRevenueAsync();
    }
}
