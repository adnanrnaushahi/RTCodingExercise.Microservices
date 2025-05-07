using System.ComponentModel.DataAnnotations;

namespace WebMVC.ViewModels
{
    public class CreatePlateViewModel
    {

        [Required(ErrorMessage = "Registration is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Registration must be between 2 and 10 characters")]
        [Display(Name = "Registration")]
        public string Registration { get; set; }

        [Required(ErrorMessage = "Letters is required")]
        [StringLength(5, MinimumLength = 1, ErrorMessage = "Letters must be between 1 and 5 characters")]
        [Display(Name = "Letters")]
        public string Letters { get; set; }

        [Required(ErrorMessage = "Numbers is required")]
        [Range(0, 99999, ErrorMessage = "Numbers must be between 0 and 99999")]
        [Display(Name = "Numbers")]
        public int Numbers { get; set; }

        [Required(ErrorMessage = "Purchase Price is required")]
        [Range(0.01, 1000000.00, ErrorMessage = "Purchase price must be greater than 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }

        [Required(ErrorMessage = "Sale Price is required")]
        [Range(0.01, 1000000.00, ErrorMessage = "Sale price must be greater than 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Sale Price")]
        public decimal SalePrice { get; set; }

        // This custom validation method ensures the 20% markup rule is followed
        public bool ValidateSalePrice()
        {
            if (PurchasePrice <= 0 || SalePrice <= 0)
                return false;

            return SalePrice >= PurchasePrice * 1.2m;
        }
    }
}