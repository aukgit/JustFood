namespace JustFood.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class QuantityConversation
    {
        public byte QuantityConversationID { get; set; }
        [Required]
        public byte QuantityTypeID { get; set; }
        [Required]
        public byte ConvertedTypeID { get; set; }
        [DisplayFormat(DataFormatString = "{0:###.#########}")]
        [Required]
        public decimal Difference { get; set; }
        
        /// <summary>
        /// QuantityTypeID
        /// </summary>
        public virtual QuantityType QuantityType { get; set; }
        /// <summary>
        /// ConvertedTypeID
        /// </summary>
        public virtual QuantityType QuantityType1 { get; set; }

    }
}
