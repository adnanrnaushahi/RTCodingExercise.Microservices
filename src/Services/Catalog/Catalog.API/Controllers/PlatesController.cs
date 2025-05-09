using System.Net;
using Catalog.API.DTO;
using Catalog.Domain.Entities;
using Catalog.Domain.Enum;
using Catalog.Domain.Interfaces;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatesController : ControllerBase
    {
        private readonly IPlateService _plateService;
        private readonly ILogger<PlatesController> _logger;

        public PlatesController(IPlateService plateService, ILogger<PlatesController> logger)
        {
            _plateService = plateService;
            _logger = logger;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PlateDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsDto<PlateDto>>> GetPlates(int pageSize = 20, int pageIndex = 0, bool orderByAsc = true)
        {
            var response = await _plateService.GetPlatesAsync(pageSize, pageIndex, orderByAsc);
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

        [HttpPost]
        [ProducesResponseType(typeof(PlateDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<PlateDto>> CreatePlate([FromBody] PlateDto createPlateDto)
        {
            try
            {
                var plate = await _plateService.CreatePlateAsync(
                    createPlateDto.Registration,
                    createPlateDto.PurchasePrice,
                    createPlateDto.SalePrice,
                    createPlateDto.Letters,
                    createPlateDto.Numbers);

                return CreatedAtAction(nameof(GetPlateById), new { id = plate.Id }, Mappers.PlateMapper.MapToPlateDto(plate));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error creating plate");
                return BadRequest(ex.Message);
            }
        }

        // Add these methods to the PlatesController in Catalog.API
        [HttpGet("filterByLetters")]
        [ProducesResponseType(typeof(PaginatedItemsDto<PlateDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsDto<PlateDto>>> FilterByLetters(string letters, int pageSize = 20, int pageIndex = 0)
        {
            var response = await _plateService.GetPlatesByLettersAsync(letters, pageSize, pageIndex);

            return Ok(new PaginatedItemsDto<PlateDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Count = response.TotalCount,
                Data = Mappers.PlateMapper.MapToPlateDto(response.Plates.ToList())
            });
        }

        [HttpGet("filterByNumbers")]
        [ProducesResponseType(typeof(PaginatedItemsDto<PlateDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsDto<PlateDto>>> FilterByNumbers(string numbers, int pageSize = 20, int pageIndex = 0)
        {
            var response = await _plateService.GetPlatesByNumbersAsync(numbers, pageSize, pageIndex);

            return Ok(new PaginatedItemsDto<PlateDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Count = response.TotalCount,
                Data = Mappers.PlateMapper.MapToPlateDto(response.Plates.ToList())
            });
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PaginatedItemsDto<PlateDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsDto<PlateDto>>> Search(string query, int pageSize = 20, int pageIndex = 0)
        {
            var response = await _plateService.SearchPlatesAsync(query, pageSize, pageIndex);

            return Ok(new PaginatedItemsDto<PlateDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Count = response.TotalCount,
                Data = Mappers.PlateMapper.MapToPlateDto(response.Plates.ToList())
            });
        }

        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(Plate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Plate>> UpdatePlateStatus(Guid id, [FromBody] PlateStatus status)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var plate = await _plateService.UpdatePlateStatusAsync(id, status);
                return Ok(plate);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plate status for {PlateId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
