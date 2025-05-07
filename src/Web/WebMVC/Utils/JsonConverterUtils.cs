using System.Text.Json;
using Catalog.Domain.Enum;
using WebMVC.ViewModels;

namespace WebMVC.Utils
{
    public static class JsonConverterUtils
    {
        public static PaginatedItemsViewModel<PlateViewModel> ToPaginatedPlatesViewModel(string jsonContent)
        {
            var plates = new List<PlateViewModel>();
            var jsonDocument = JsonDocument.Parse(jsonContent);
            var root = jsonDocument.RootElement;

            var pageIndexValue = root.GetProperty("pageIndex").GetInt32();
            var pageSizeValue = root.GetProperty("pageSize").GetInt32();
            var totalCount = root.GetProperty("count").GetInt64();
            var dataArray = root.GetProperty("data").EnumerateArray();

            foreach (var item in dataArray)
            {
                plates.Add(ToPlateViewModel(item));
            }

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSizeValue);

            return new PaginatedItemsViewModel<PlateViewModel>
            {
                PageIndex = pageIndexValue,
                PageSize = pageSizeValue,
                TotalItems = totalCount,
                TotalPages = totalPages,
                Data = plates
            };
        }

        public static PlateViewModel ToPlateViewModel(JsonElement element)
        {
            return new PlateViewModel
            {
                Id = element.GetProperty("id").GetGuid(),
                Registration = element.GetProperty("registration").GetString(),
                PurchasePrice = element.GetProperty("purchasePrice").GetDecimal(),
                SalePrice = element.GetProperty("salePrice").GetDecimal(),
                Letters = element.GetProperty("letters").GetString(),
                Numbers = element.GetProperty("numbers").GetInt32(),
                Status = (PlateStatus)element.GetProperty("status").GetInt32(),
            };
        }

        public static PlateViewModel ToPlateViewModel(string jsonContent)
        {
            var jsonDocument = JsonDocument.Parse(jsonContent);
            return ToPlateViewModel(jsonDocument.RootElement);
        }
    }
}
