﻿using Catalog.Domain.Enum;

namespace WebMVC.ViewModels
{
    public class PlateViewModel
    {
        public Guid Id { get; set; }
        public string Registration { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public string Letters { get; set; }
        public int Numbers { get; set; }
        public PlateStatus Status { get; set; }
    }
}
