using System;
using JustFood.Models;
using JustFood.Modules.Query;
using JustFood.Modules.StaticContents;

namespace JustFood.Modules.Extensions {
    public class InventoryExtension {

        #region Declares

        readonly UserInfo userinfo = new UserInfo();

        #endregion

        /// <summary>
        /// Call this method when adding product to inventory.
        /// Add product info into the InventoryIn table and then ++ then quantity in the Inventory Table By product Extension class..
        /// Modify Account Balance for product bought.
        /// Verify account balance info.
        /// Add account balance to db.
        /// Create InventoryIn
        /// Create Product
        /// Add those to database.
        /// Just not save. Save it when all successful.
        /// </summary>
        /// <param name="db">Pass the dbContext</param>
        /// <param name="accountBalance"></param>
        /// <returns></returns>
        public bool InventoryAdd(JustFoodDBEntities db, AccountBalance accountBalance) {
            bool result = true;
            accountBalance.Dated = DateTime.Now;
            accountBalance.AddBy = userinfo.GetUserSession().UserID;
            accountBalance.IsBoughtProduct = true;
            accountBalance.IsAddedMoney = false;
            accountBalance.IsExpense = true;

            if (accountBalance.Amount < 0 || accountBalance.AddedQuantity < 1) {
                Statics.ErrorCollection.Add("Amount", "Amount or Quantity can't be negative.");
                result = false;
            }
            //checking if quantity type is valid.
            if (accountBalance.QtyType == null || db.QuantityTypes.Find(accountBalance.QtyType) == null) {
                Statics.ErrorCollection.Add("QtyType", "Quantity type is not valid , please select a valid quantity type.");
                result = false;
            }
            var quantity = accountBalance.AddedQuantity;
            var unitprice = (accountBalance.Amount / (double)quantity);
            var qtyExtension = new QuantityTypeExtension();
            unitprice = Math.Round(unitprice, 2);
            int userid = userinfo.GetUserID();

            // check if quantity mismatch.
            decimal multiplier = 0;
            var mismatch = qtyExtension.Mismatch((int)accountBalance.CategoryProduct, (byte)accountBalance.QtyType, out multiplier);
            if (!mismatch) {
                var invtIn = new InventoryIn {
                    CategoryID = (int)accountBalance.CategoryProduct,
                    Quantity = (decimal)quantity,
                    UnitPrice = unitprice,
                    AddedBy = userid,
                    QtyType = (byte)accountBalance.QtyType,
                    Dated = DateTime.Now
                };
                var productExt = new ProductExtension();
                //add inventory product in.
                db.InventoryIns.Add(invtIn);
                accountBalance.AddedQuantity = accountBalance.AddedQuantity * multiplier;
                accountBalance.Amount = accountBalance.Amount * -1; //expense
                db.AccountBalances.Add(accountBalance);
                productExt.AddProduct(accountBalance, db);
            }
            return result;
        }

       
    }
}