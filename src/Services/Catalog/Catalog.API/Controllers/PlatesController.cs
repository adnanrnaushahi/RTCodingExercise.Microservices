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
        public async Task<ActionResult<PaginatedItemsDto<PlateDto>>> GetPlates(int pageSize = 20, int pageIndex = 0)
        {
            var response = await _plateService.GetPlatesAsync(pageSize, pageIndex);
            return Ok(new PaginatedItemsDto<PlateDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Count = response.TotalCount,
                Data = Mappers.PlateMapper.MapToPlateDto(response.Plates.ToList())
            });
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
