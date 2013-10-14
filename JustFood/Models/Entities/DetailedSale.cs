using System.ComponentModel.DataAnnotations;

namespace JustFood.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    
    public partial class DetailedSale
    {
        public long DetailedSaleID { get; set; }
        public int SaleID { get; set; }
        public int CategoryID { get; set; }
        public int SellingPrice { get; set; }
        public Nullable<int> Discount { get; set; }
        public string Note { get; set; }
        public int SoldAt { get; set; }
        public System.DateTime Time { get; set; }

        public int UserID { get; set; }
        /// <summary>
        /// True when add a record of Discard
        /// </summary>
        [DefaultValue(false)]
        public bool IsDiscard { get; set; }
        /// <summary>
        /// True when last sold object is marked as discarded.
        /// </summary>
        [DefaultValue(false)]
        public bool IsMarkedDiscard { get; set; }

        [MaxLength(20)]
        public string DiscountPerson { get; set; }

        [MaxLength(30)]
        public string DiscountNumber { get; set; }

        public virtual Category Category { get; set; }
        public virtual Sale Sale { get; set; }
        
        /// <summary>
        /// How many quantities sold. If it is discarded then -1 or whatever the previous number is.
        /// </summary>
        public short Qty { get; set; }


        /// <summary>
        /// user who processed it.
        /// </summary>
        public virtual User User { get; set; }

    }
}
