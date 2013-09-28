namespace JustFood.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class Code {
        public int CodeID { get; set; }
        [DisplayName("Is Used")]
        public bool IsUsed { get; set; }
        [DisplayName("Guid Code")]
        public System.Guid GuidCode { get; set; }
        [DisplayName("Code")]
        public string Code1 { get; set; }
        [DisplayName("Is Owner")]
        public bool IsOwner { get; set; }
        [DisplayName("Is Admin")]
        public bool IsAccessToAdmin { get; set; }
        [DisplayName("Is Employee(Access to Sales Only)")]
        public bool IsEmployee { get; set; }
        public Nullable<decimal> Salary { get; set; }
        public Nullable<decimal> Percentage { get; set; }
    }
}
