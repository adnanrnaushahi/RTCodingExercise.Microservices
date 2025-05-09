using Catalog.Domain.Entities;
using Catalog.Domain.Enum;

namespace Catalog.Domain.Interfaces
{
    public interface IPlateService
    {
        Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true);
        Task<Plate?> GetPlateByIdAsync(Guid id);
        Task<Plate> CreatePlateAsync(string registration, decimal purchasePrice, decimal salePrice, string letters, int numbers);
        Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesByLettersAsync(string letters, int pageSize, int pageIndex);
        Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesByNumbersAsync(string numbers, int pageSize, int pageIndex);
        Task<(IEnumerable<Plate> Plates, int TotalCount)> SearchPlatesAsync(string query, int pageSize, int pageIndex);
        Task<Plate> UpdatePlateStatusAsync(Guid plateId, PlateStatus newStatus);
    }
}
