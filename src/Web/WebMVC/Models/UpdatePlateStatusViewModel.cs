using Catalog.Domain.Enum;

namespace WebMVC.ViewModels
{
    public class UpdatePlateStatusViewModel
    {
        public Guid PlateId { get; set; }
        public string Registration { get; set; }
        public PlateStatus NewStatus { get; set; }
    }
}