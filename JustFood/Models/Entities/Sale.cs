using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace JustFood.Models {
    public class Sale {
        public Sale() {
            CategoryWiseSolds = new HashSet<CategoryWiseSold>();
            DailyStocks = new HashSet<DailyStock>();
            DetailedSales = new HashSet<DetailedSale>();
        }


        public int SaleID { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<double> Discount { get; set; }
        [DisplayName("Total Sold(money)")]
        public int TotalSold { get; set; }
        [DisplayName("Total Less(money)")]
        public Nullable<double> TotalLess { get; set; }
        /// <summary>
        /// Assumed achieved money from sales.
        /// </summary>
        [DisplayName("Total Assumed Acheived(money)")]
        public decimal TotalAcheived { get; set; }
        [DisplayName("Expenses")]
        public decimal OtherExpenses { get; set; }
        public bool IsDividedAmongPartners { get; set; }
        public int TotalWastages { get; set; }
        public int TotalDiscountNumber { get; set; }
        public string Noted { get; set; }
        [DisplayName("Problem")]
        public bool AnyProblem { get; set; }
        public int DiscardSales { get; set; }
        public bool IsDiscardsChecked { get; set; }
        public double InHandCashChange { get; set; }

        /// <summary>
        /// Checks if employee counts the inventory or not before the sale.
        /// </summary>
        public bool IsInventorySet { get; set; }

        /// <summary>
        /// Actual Collection from the employee.
        /// </summary>
        public decimal ActualAcheivedFromEmployee { get; set; }
        /// <summary>
        /// After actual collection calculate actual profit/loss
        /// </summary>
        public decimal ActualLossProfitAfterCollection { get; set; }


        /// <summary>
        /// This one is for the optimization of the query pressure in ViewSummarySale
        /// </summary>
        public virtual ICollection<CategoryWiseSold> CategoryWiseSolds { get; set; }
        public virtual ICollection<DailyStock> DailyStocks { get; set; }
        public virtual ICollection<DetailedSale> DetailedSales { get; set; }
    }
}