using WebMVC.Services;

namespace WebMVC.Controllers
{
    public class PlatesController : Controller
    {
        private readonly IPlateService _plateService;
        private readonly ILogger<PlatesController> _logger;
        private const int DefaultPageSize = 20;

        public PlatesController(IPlateService plateService, ILogger<PlatesController> logger)
        {
            _plateService = plateService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageIndex = 0)
        {            
            var plates = await _plateService.GetAllPlatesAsync(DefaultPageSize, pageIndex);
            return View(plates);            
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var plate = await _plateService.GetPlateByIdAsync(id);
                return View(plate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plate details");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
