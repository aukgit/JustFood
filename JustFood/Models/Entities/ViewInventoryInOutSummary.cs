
namespace JustFood.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ViewInventoryInOutSummary
    {
        public int CategoryID { get; set; }
        public Nullable<decimal> ProductIn { get; set; }
        public Nullable<decimal> DiscardedProduct { get; set; }
        public Nullable<decimal> ProductLeft { get; set; }
    }
}
