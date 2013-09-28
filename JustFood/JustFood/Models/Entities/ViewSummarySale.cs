namespace JustFood.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ViewSummarySale
    {
        public Nullable<int> SaleID { get; set; }
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public double SoldPrice { get; set; }
        public double InventoryQuantity { get; set; }
        public Nullable<int> SoldQuantity { get; set; }
        public Nullable<int> Discounted { get; set; }
        public Nullable<int> CashInHand { get; set; }
    }
}
