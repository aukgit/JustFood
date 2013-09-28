namespace JustFood.Models {
    public class InventoryOutConfig {
        public int InventoryOutConfigID { get; set; }
        public int CategoryID { get; set; }
        public int DiscardItemCategory { get; set; }
        public decimal PerSaleQuantity { get; set; }
        public byte QtyType { get; set; }

        public virtual Category Category { get; set; }
        public virtual Category Category1 { get; set; }
        public virtual QuantityType QuantityType { get; set; }
    }
}