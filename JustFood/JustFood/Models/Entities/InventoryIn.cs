using System.ComponentModel.DataAnnotations;
namespace JustFood.Models {
    public class InventoryIn {
        public long InventoryInID { get; set; }
        public int CategoryID { get; set; }
        [DisplayFormat(DataFormatString = "{0:###.00##}")]
        public decimal Quantity { get; set; }
        public double UnitPrice { get; set; }
        public int AddedBy { get; set; }
        public bool IsAddedBySelling { get; set; }
        public byte QtyType { get; set; }
        [Required]
        public System.DateTime Dated { get; set; }

        public virtual Category Category { get; set; }
        public virtual QuantityType QuantityType { get; set; }
        public virtual User User { get; set; }
    }
}