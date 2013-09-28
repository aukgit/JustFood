using System;

namespace JustFood.Models {
    public class SalaryPaid {
        public long SalaryPaidID { get; set; }
        public int UserID { get; set; }
        public DateTime SalaryOfDated { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public double Paid { get; set; }
        public double Salary { get; set; }
        public DateTime PaidDate { get; set; }

        public virtual User User { get; set; }
    }
}