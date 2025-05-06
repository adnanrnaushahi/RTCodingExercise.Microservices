namespace Catalog.API.Controllers
{
    public class PlatesController : ControllerBase
    {
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
