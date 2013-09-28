namespace JustFood.Models {
    public partial class CategoryWiseSold {
        public System.Guid CategoryWiseSoldID { get; set; }
        public int SaleID { get; set; }
        public int CategoryID { get; set; }
        public long Quantity { get; set; }

        public virtual Category Category { get; set; }
        public virtual Sale Sale { get; set; }
    }
}
