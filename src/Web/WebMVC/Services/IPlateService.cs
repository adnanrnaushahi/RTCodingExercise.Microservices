using Catalog.Domain.Enum;
using Catalog.Domain.Models;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public interface IPlateService
    {
        Task<PaginatedItemsViewModel<PlateViewModel>> GetAllPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true, PlateStatus? status = null);
        Task<PlateViewModel> GetPlateByIdAsync(Guid id);
        Task<PlateViewModel> CreatePlateAsync(CreatePlateViewModel model);
        Task<PaginatedItemsViewModel<PlateViewModel>> FilterByLettersAsync(string letters, int pageSize, int pageIndex);
        Task<PaginatedItemsViewModel<PlateViewModel>> FilterByNumbersAsync(string numbers, int pageSize, int pageIndex);
        Task<PaginatedItemsViewModel<PlateViewModel>> SearchPlatesAsync(string query, int pageSize, int pageIndex);
        Task<PlateViewModel> UpdatePlateStatusAsync(Guid id, PlateStatus newStatus);
        Task<RevenueViewModel> GetTotalRevenueAsync();
    }
}
