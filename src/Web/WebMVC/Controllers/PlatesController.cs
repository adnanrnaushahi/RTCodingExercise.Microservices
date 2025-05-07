using WebMVC.Services;
using WebMVC.ViewModels;

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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePlateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.SalePrice < model.PurchasePrice * 1.2m)
                    {
                        // If the 20% markup rule is not met, add a model error
                        ModelState.AddModelError("SalePrice", "Sale price must include at least 20% markup over purchase price.");
                        return View(model);
                    }

                    await _plateService.CreatePlateAsync(model);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating plate");
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }
    }
}
