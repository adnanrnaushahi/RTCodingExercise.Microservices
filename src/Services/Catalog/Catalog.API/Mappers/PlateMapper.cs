using Catalog.API.DTO;

namespace Catalog.API.Mappers
{
    public static class PlateMapper
    {
        public static PlateDto MapToPlateDto(Domain.Entities.Plate plate)
        {
            return new PlateDto
            {
                Id = plate.Id,
                Registration = plate.Registration,
                PurchasePrice = plate.PurchasePrice,
                SalePrice = plate.SalePrice,
                Letters = plate.Letters,
                Numbers = plate.Numbers,
                Status = plate.Status
            };
        }

        public static IEnumerable<PlateDto> MapToPlateDto(IEnumerable<Domain.Entities.Plate> plates)
        {
            var result = new List<PlateDto>();

            foreach (var plate in plates)
            {
                result.Add(MapToPlateDto(plate));
            }

            return result;
        }
    }
}
