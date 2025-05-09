using Catalog.Domain.Enum;
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

        public async Task<IActionResult> Index(int pageIndex = 0, string sortOrder = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.PriceSortParam = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            GetTodayRevenue();
            var plates = await _plateService.GetAllPlatesAsync(DefaultPageSize, pageIndex, sortOrder == "price_desc" ? false : true);
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
                    if (!model.ValidateSalePrice())
                    {
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

        public async Task<IActionResult> ReservePlate(Guid id)
        {
            try
            {
                var currentUser = User.Identity?.Name ?? "System";
                await _plateService.UpdatePlateStatusAsync(id, PlateStatus.Reserved);
                TempData["SuccessMessage"] = "Plate successfully reserved";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving plate {PlateId}", id);
                TempData["ErrorMessage"] = "Error reserving plate";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> MakeAvailable(Guid id)
        {
            try
            {
                var currentUser = User.Identity?.Name ?? "System";
                await _plateService.UpdatePlateStatusAsync(id, PlateStatus.Available);
                TempData["SuccessMessage"] = "Plate marked as available";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making plate {PlateId} available", id);
                TempData["ErrorMessage"] = "Error updating plate status";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> MarkAsSold(Guid id)
        {
            try
            {
                var currentUser = User.Identity?.Name ?? "System";
                await _plateService.UpdatePlateStatusAsync(id, PlateStatus.Sold);

                GetTodayRevenue();
                TempData["SuccessMessage"] = "Plate marked as sold";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking plate {PlateId} as sold", id);
                TempData["ErrorMessage"] = "Error updating plate status";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Available(int pageIndex = 0, string sortOrder = "")
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.PriceSortParam = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";

            try
            {
                GetTodayRevenue();
                var plates = await _plateService.GetAllPlatesAsync(DefaultPageSize, pageIndex, sortOrder == "price_desc" ? false : true, PlateStatus.Available);

                return View(plates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available plates");
                TempData["ErrorMessage"] = "Error retrieving available plates. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async void GetTodayRevenue()
        {
            // Get total revenue and set in TempData
            var revenue = await _plateService.GetTotalRevenueAsync();
            TempData["TotalRevenue"] = revenue.TotalSalePrice;
            TempData["AverageProfitMargin"] = revenue.AverageProfitMargin;
        }
    }
}
