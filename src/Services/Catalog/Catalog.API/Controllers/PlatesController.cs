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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PlateDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PlateDto>> GetPlateById(Guid id)
        {
            var plate = await _plateService.GetPlateByIdAsync(id);

            if (plate == null)
                return NotFound();

            return Ok(Mappers.PlateMapper.MapToPlateDto(plate));
        }
    }
}
