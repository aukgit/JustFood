using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JustFood.Models {
    public class AccountBalanceSearch {
        [Range(1, 12)]
        public int? Month { get; set; }

        [Range(2000, 2200)]
        public int? Year { get; set; }

        [DisplayName("By User")]
        public string ByUser { get; set; }
    }
}