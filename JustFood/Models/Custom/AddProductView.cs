
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JustFood.Models {
    public class AddProductView {
        
        [DisplayName("Category")]
        [Required]
        public int CategoryID { get; set; }
        
        [Required]
        public int Quantity { get; set; }

        [Required]
        [DisplayName("Quantity Type")]
        public byte QuantityType { get; set; }
        
        [Required]
        public int Cost { get; set; }
    }
}