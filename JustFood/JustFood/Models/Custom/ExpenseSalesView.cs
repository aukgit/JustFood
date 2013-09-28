

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace JustFood.Models {
    public class ExpenseSalesView {
        [Required]
        public int  SaleID { get; set; }
        [DisplayName("Type of Expense")]
        [Required]
        public int ExpenseCategoryID { get; set; }
        [Required,Range(0,5000)]
        public int Amount { get; set; }
        [Required,MaxLength(50)]
        [DisplayName("*Why ?")]
        public string Note { get; set; }
    }
}