using WebMVC.Services;

namespace WebMVC.Controllers
{
    public class PlatesController : Controller
    {
        private readonly IPlateService _plateService;

        public PlatesController(IPlateService plateService)
        {
            _plateService = plateService;
           
        }

        public async Task<IActionResult> Index()
        {
            var plates = await _plateService.GetAllPlatesAsync();
            return View(plates);
        }
    }
}
