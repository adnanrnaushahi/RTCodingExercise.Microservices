using WebMVC.Services;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPlateService _plateService;
        private readonly ILogger<PlatesController> _logger;
        private const int DefaultPageSize = 20;

        public SearchController(IPlateService plateService, ILogger<PlatesController> logger)
        {
            _plateService = plateService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View(new SearchViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SearchResults(SearchViewModel model, int pageIndex = 0)
        {
            if (string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            PaginatedItemsViewModel<PlateViewModel> results;

            try
            {
                switch (model.SearchType)
                {
                    case SearchType.Letters:
                        results = await _plateService.FilterByLettersAsync(model.SearchTerm, DefaultPageSize, pageIndex);
                        break;
                    case SearchType.Numbers:
                        results = await _plateService.FilterByNumbersAsync(model.SearchTerm, DefaultPageSize, pageIndex);
                        break;
                    case SearchType.Any:
                    default:
                        results = await _plateService.SearchPlatesAsync(model.SearchTerm, DefaultPageSize, pageIndex);
                        break;
                }

                ViewBag.SearchTerm = model.SearchTerm;
                ViewBag.SearchType = model.SearchType;

                return View(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for plates");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
