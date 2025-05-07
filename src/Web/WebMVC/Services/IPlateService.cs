using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public interface IPlateService
    {
        Task<PaginatedItemsViewModel<PlateViewModel>> GetAllPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true);
        Task<PlateViewModel> GetPlateByIdAsync(Guid id);
        Task<PlateViewModel> CreatePlateAsync(CreatePlateViewModel model);
    }
}
