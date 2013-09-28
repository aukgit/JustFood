using System.Collections.Generic;
using System.Linq;
using System;

namespace JustFood.Models {
    public class SaleDayView {
        readonly JustFoodDBEntities db = new JustFoodDBEntities();
        readonly int saleId = -1;
        bool summaryExist = false;
        List<ViewSummarySale> summary;
        public DateTime SaleDated { get; set; }

        public SaleDayView(int SaleID) { saleId = SaleID; }


        /// <summary>
        /// Call it after executing GetSummary Method.
        /// </summary>
        public bool IsSummaryExist { get { return summaryExist; } }
        public SaleDayView(DateTime date) { 
            var date2 = date.Date;
            var sale = db.Sales.FirstOrDefault(n => n.Date == date2);
            if (sale != null) {
                saleId = sale.SaleID;
            } else {
                throw new Exception("Date is not valid.");
            }
        }

        public List<ViewInventorySalable> Salables {
            get { return db.ViewInventorySalables.ToList(); }
        }

        public List<ViewSummarySale> GetSummary() {
            if (summary != null) {
                return summary;
            }
            summary = db.ViewSummarySales.Where(n => n.SaleID == saleId)
                            .ToList();
            if (summary != null && summary.Count > 0) {
               summaryExist = true;
            }
            return summary;
        }

        public Sale Sale { get; set; }
        public DiscountView DiscountView { get; set; }
        public ExpenseSalesView ExpenseSalesView { get; set; }
        public int SaleID { get { return saleId; } }
        public Requisition Requisition { get; set; }
        public AddProductView AddProductView { get; set; }

    }
}