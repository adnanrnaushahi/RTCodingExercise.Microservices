using Catalog.Domain.Enum;

namespace Catalog.Domain.Entities
{
    public class Plate
    {
        public Guid Id { get; set; }

        public string? Registration { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public string? Letters { get; set; }

        public int Numbers { get; set; }

        public PlateStatus Status { get; set; } = PlateStatus.Available;

        public Plate(string registration, decimal purchasePrice, decimal salePrice, string letters, int numbers)
        {
            Id = Guid.NewGuid();
            Registration = registration;
            PurchasePrice = purchasePrice;
            SalePrice = salePrice;
            Letters = letters;
            Numbers = numbers;
            Status = PlateStatus.Available;
        }
    }
}