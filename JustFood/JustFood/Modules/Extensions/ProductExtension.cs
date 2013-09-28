using System.Linq;
using JustFood.Areas.Admin.Controllers;
using JustFood.Models;

namespace JustFood.Modules.Extensions {
    public class ProductExtension {

       
        /// <summary>
        /// To add product to inventory : do not call this method . Instead call InventoryExtention Invetory Add to do all the work.
        /// Add ++ quantity to inventory of the specific product by the account balance info.
        /// </summary>
        /// <param name="accountbalance"></param>
        /// <param name="dbx"></param>
        public void AddProduct(AccountBalance accountbalance, JustFoodDBEntities dbx) {
            if (!accountbalance.IsBoughtProduct) {
                return;
            }
            if (accountbalance.AddedQuantity > 0) {
                Inventory inventory = dbx.Inventories.FirstOrDefault(n => n.CategoryID == accountbalance.CategoryProduct);
                if (inventory != null) {
                    inventory.Quantity += (int)accountbalance.AddedQuantity;
                } else {
                    // if null
                    var invController = new InventoryController();
                    invController.Index();
                    inventory = dbx.Inventories.FirstOrDefault(n => n.CategoryID == accountbalance.CategoryProduct);
                    if (inventory != null) {
                        inventory.Quantity += (int)accountbalance.AddedQuantity;
                    }
                }
               // dbx.InventoryIns.Add(invtIn);
            }
        }
    }
}