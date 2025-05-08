using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public interface IPlateService
    {
        Task<PaginatedItemsViewModel<PlateViewModel>> GetAllPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true);
        Task<PlateViewModel> GetPlateByIdAsync(Guid id);
        Task<PlateViewModel> CreatePlateAsync(CreatePlateViewModel model);
        Task<PaginatedItemsViewModel<PlateViewModel>> FilterByLettersAsync(string letters, int pageSize, int pageIndex);
        Task<PaginatedItemsViewModel<PlateViewModel>> FilterByNumbersAsync(string numbers, int pageSize, int pageIndex);
        Task<PaginatedItemsViewModel<PlateViewModel>> SearchPlatesAsync(string query, int pageSize, int pageIndex);
    }
}
