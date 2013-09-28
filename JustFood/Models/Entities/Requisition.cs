using System.ComponentModel;
namespace JustFood.Models {
    public class Requisition {
        public int RequisitionID { get; set; }
        [DisplayName("Type of Requisition")]
        public int CategoryID { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Quantity Type Value
        /// </summary>
        [DisplayName("Quantity Type")]
        public byte QtyType { get; set; }


        public bool IsAutoAdded { get; set; }
        [DisplayName("Is In Process")]
        public bool IsInProcess { get; set; }
        /// <summary>
        /// Processing By whom.
        /// </summary>
        [DisplayName("By Whom")]
        public int? ByWhomUser { get; set; }
        [DisplayName("Is Done")]
        public bool IsDone { get; set; }
        /// <summary>
        /// Added by whom.
        /// </summary>
        [DisplayName("Added By")]
        public int AddedBy { get; set; }

        /// <summary>
        /// List of Quantity types
        /// </summary>
        public virtual QuantityType QuantityType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}