namespace JustFood.Models {
    public class Inventory {
        public int InventoryID { get; set; }
        public int CategoryID { get; set; }
        public double Quantity { get; set; }
        public int LastEditedby { get; set; }
        public double SoldPrice { get; set; }
        public byte QtyType { get; set; }

        public virtual Category Category { get; set; }
        public virtual QuantityType QuantityType { get; set; }
        public virtual User User { get; set; }
    }
}