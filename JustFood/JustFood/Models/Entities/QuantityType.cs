namespace JustFood.Models
{
    using System;
    using System.Collections.Generic;


    public partial class QuantityType {
        public QuantityType() {
            this.AccountBalances = new HashSet<AccountBalance>();
            this.Categories = new HashSet<Category>();
            this.DailyStocks = new HashSet<DailyStock>();
            this.Inventories = new HashSet<Inventory>();
            this.InventoryIns = new HashSet<InventoryIn>();
            this.InventoryOuts = new HashSet<InventoryOut>();
            this.InventoryOutConfigs = new HashSet<InventoryOutConfig>();
            this.QuantityConversations = new HashSet<QuantityConversation>();
            this.QuantityConversations1 = new HashSet<QuantityConversation>();
            this.Requisitions = new HashSet<Requisition>();
        }

        public byte QuantityTypeID { get; set; }
        public string QtyType { get; set; }


        public virtual ICollection<AccountBalance> AccountBalances { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<DailyStock> DailyStocks { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<InventoryIn> InventoryIns { get; set; }
        public virtual ICollection<InventoryOut> InventoryOuts { get; set; }
        public virtual ICollection<InventoryOutConfig> InventoryOutConfigs { get; set; }
        /// <summary>
        /// QuantityTypeID
        /// </summary>
        public virtual ICollection<QuantityConversation> QuantityConversations { get; set; }
        /// <summary>
        /// ConvertedTypeID
        /// </summary>
        public virtual ICollection<QuantityConversation> QuantityConversations1 { get; set; }

        public virtual ICollection<Requisition> Requisitions { get; set; }

    }
}
