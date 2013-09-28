namespace JustFood.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ViewSummaryAccountBalance
    {
        public string AccountOfLog { get; set; }
        public string AccountOfName { get; set; }
        public Nullable<double> Balance { get; set; }
        public Nullable<int> AccountOf { get; set; }
    }
}
