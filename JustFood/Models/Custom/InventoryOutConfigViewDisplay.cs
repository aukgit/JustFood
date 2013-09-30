
using System.ComponentModel.DataAnnotations;
namespace JustFood.Models.Custom {
    /// <summary>
    /// Passing ajax or json request to controller from Index of InventoryConfig to save the form into the database.
    /// </summary>
    public class InventoryOutConfigViewDisplay {
        [Required]
        public int InventoryID { get; set; }
        /// <summary>
        /// Category Id of that currently selected inventory item.
        /// Connect with db's [CategoryID] field.
        /// </summary>
        [Required]
        public int CategoryID { get; set; }

        /// <summary>
        /// Pepsi or coke doesn't need a discard category.
        /// For say burguer category needs to deduct from breads, Sauce etc...
        /// Each of those will be discard category.
        /// Connect with db's [DiscardItemCategory] field.
        /// </summary>
        public int DiscardCategoryID { get; set; }

        /// <summary>
        /// Quantity of the deduction on that specific DiscardCategoryID.
        /// Connection with database's [PerSaleQuantity] field.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Quantity type of the deduction category item. 
        /// It should be validated before saving to the database.
        /// Connection with database's [QtyType] field.
        /// </summary>
        public byte QuantityTypeID { get; set; }

        /// <summary>
        /// Inventory config id to detect it is saved before or not.
        /// If it is -1 then not saved. If other values then saved now update it.
        /// </summary>
        public int InventoryOutConfigID { get; set; }

        /// <summary>
        /// Returns true if this is a case to add the item to the database.
        /// </summary>
        public bool IsDatabaseInsert { get { return InventoryOutConfigID == -1; } }

    }
}