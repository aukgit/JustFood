

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JustFood.Models {
    public class DiscountView {
        [Required]
        public int SaleID { get; set; }
        [Required]
        [DisplayName("Category")]
        public int DiscountCategoryID { get; set; }
        /// <summary>
        /// Sold Price
        /// </summary>
        [DisplayName("Sold Price")]
        [Required, Range(1, 3000)]
        public int Amount { get; set; }
        [Required,MaxLength(20)]
        public String Phone { get; set; }
        [Required,MaxLength(30)]
        public String Name { get; set; }

        [MaxLength(50)]
        public String Notes { get; set; }
        
    }
}