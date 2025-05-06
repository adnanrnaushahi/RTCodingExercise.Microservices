using System.Net;
using Catalog.API.DTO;
using Catalog.Domain.Interfaces;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatesController : ControllerBase
    {
        private readonly IPlateService _plateService;

        public PlatesController(IPlateService plateService)
        {
            _plateService = plateService;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PlateDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<PlateDto>>> GetPlates()
        {
            var plates = await _plateService.GetAllPlatesAsync();
            return Ok(Mappers.PlateMapper.MapToPlateDto(plates));
        }
    }
}
