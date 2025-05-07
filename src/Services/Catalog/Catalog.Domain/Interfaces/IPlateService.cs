
using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces
{
    public interface IPlateService
    {
        Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesAsync(int pageSize, int pageIndex);

        Task<Plate?> GetPlateByIdAsync(Guid id);

        Task<Plate> CreatePlateAsync(string registration, decimal purchasePrice, decimal salePrice, string letters, int numbers);
    }
}
