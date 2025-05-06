namespace Catalog.Domain.Entities
{
    public class PlateDetail
    {
        public Guid Id { get; set; }

        public Guid? PlateId { get; set; }

        public bool IsAvialable { get; set; }
    }
}