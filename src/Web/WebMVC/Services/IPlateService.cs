using WebMVC.Models;

namespace WebMVC.Services
{
    public interface IPlateService
    {
        Task<IEnumerable<PlateViewModel>> GetAllPlatesAsync();
    }
}
