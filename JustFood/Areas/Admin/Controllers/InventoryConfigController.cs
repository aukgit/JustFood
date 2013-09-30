using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustFood.Models;
using JustFood.Models.Custom;
using JustFood.Modules.Extensions;
using System.ComponentModel.DataAnnotations;

namespace JustFood.Areas.Admin.Controllers {
    public class InventoryConfigController : Controller {
        readonly JustFoodDBEntities db = new JustFoodDBEntities();

        class CustomJsonResult {
            /// <summary>
            /// Operation status 
            /// True: Successful.
            /// False: Failed.
            /// </summary>
            [Required]
            public bool Status { get; set; }

            /// <summary>
            /// Operation message when failed.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// InevntoryOutConfigID
            /// </summary>
            [Required]
            public int ConfigId { get; set; }
        }

        public ActionResult Index() {
            var inventorySalables = db.ViewInventorySalables.ToList();
            ViewBag.InventoryOut = db.InventoryOutConfigs.Include(n => n.Category)
                                     .Include(n => n.Category1).OrderByDescending(n => n.InventoryOutConfigID).ToList();
            ViewBag.Categories = db.Categories.Where(n=> !n.IsExpense && !n.IsRelatedToSalary).ToList();
            ViewBag.QuantityTypes = db.QuantityTypes.ToList();
            return View(inventorySalables);
        }

        public JsonResult Add(InventoryOutConfigViewDisplay inventory) {
            var result = new CustomJsonResult {
                Status = true,
                Message = "Successful",
                ConfigId = -1
            };
            if (inventory != null) {
                var quantityTypeExt = new QuantityTypeExtension();
                decimal multiplier = 0;
                /* *
                 * Passing DiscardCategoryID because Category id is for that inventory item category
                 * which is irrelevent here. We are deduction small products by DiscardCategoryID 
                 * and so we are passing DiscardCategoryID instead of CategoryID
                 * */
                if (!quantityTypeExt.Mismatch(inventory.DiscardCategoryID, inventory.QuantityTypeID, out multiplier,false)) {
                    var inventoryOutConfig = new InventoryOutConfig() {
                        CategoryID = inventory.CategoryID, //category of that inventory
                        DiscardItemCategory = inventory.DiscardCategoryID, //discarding category when item is sold. For say burguer category needs to deduct from breads.
                        QtyType = inventory.QuantityTypeID,
                        PerSaleQuantity = inventory.Quantity //keep the same quantity and id deduct by mathematics multiplication in the real time selling not here.
                    };
                    db.InventoryOutConfigs.Add(inventoryOutConfig);
                    db.SaveChanges();

                    //updating configid.
                    result.ConfigId = inventoryOutConfig.InventoryOutConfigID;
                    
                    return Json(result);
                }
            }
           
            result.Status = false;
            result.Message = "Either quantity type validation failed or server internal error. Try again with different quantity type.";

            return Json(result);
        }


        /// <summary>
        /// Is for either saving new or modify exisiting record.
        /// To save new record method will call add method based on the paramter on configId = -1.
        /// </summary>
        /// <param name="inventoryDisplay"></param>
        /// <returns></returns>
        public JsonResult Save(InventoryOutConfigViewDisplay inventoryDisplay) {
            var result = new CustomJsonResult {
                Status = true,
                Message = "Successful",
                ConfigId = -1
            };


            if (inventoryDisplay.IsDatabaseInsert) {
                return Add(inventoryDisplay);
            } else {
                // saving
                // assuming that information exist in the database
                // now lets verify.
                // CategoryId is verifing for double checking if inevntory categoryid are same with this InventoryOutConfigID.
                // Which must be same.
                var inventoryOutConfig = db.InventoryOutConfigs
                                           .FirstOrDefault(n =>    n.InventoryOutConfigID == inventoryDisplay.InventoryOutConfigID
                                                                && n.CategoryID == inventoryDisplay.CategoryID);

                if (inventoryOutConfig != null) {
                    // exist in the database.
                    // now first check for quantity mismatch.
                    var quantityTypeExt = new QuantityTypeExtension();
                    decimal multiplier = 0;
                    /* *
                     * Passing DiscardCategoryID because Category id is for that inventory item category
                     * which is irrelevent here. We are deduction small products by DiscardCategoryID 
                     * and so we are passing DiscardCategoryID instead of CategoryID
                     * */
                    if (!quantityTypeExt.Mismatch(inventoryDisplay.DiscardCategoryID, inventoryDisplay.QuantityTypeID, out multiplier,false)) {
                        //if it doesn't mismatch.
                        inventoryOutConfig.DiscardItemCategory = inventoryDisplay.DiscardCategoryID;
                        inventoryOutConfig.PerSaleQuantity = inventoryDisplay.Quantity;
                        inventoryOutConfig.QtyType = inventoryDisplay.QuantityTypeID;
                        db.SaveChanges();
                        return Json(result);
                    }
                }
            }
            result.Status = false;
            result.Message = "Either quantity type validation failed or server internal error. Try again with different quantity type.";
            return Json(result);

        }

        public JsonResult Remove(InventoryOutConfigViewDisplay inventoryDisplay) {
            var result = new CustomJsonResult {
                Status = true,
                Message = "Successful",
                ConfigId = -1
            };
            var inventoryOutConfig = db.InventoryOutConfigs
                                       .FirstOrDefault(n => n.InventoryOutConfigID == inventoryDisplay.InventoryOutConfigID 
                                                         && n.CategoryID == inventoryDisplay.CategoryID);
            if (inventoryOutConfig != null) {
                db.InventoryOutConfigs.Remove(inventoryOutConfig);
                db.SaveChanges();
                return Json(result);

            }

            result.Status = false;
            result.Message = "Sorry for the inconvience can't remove this record.";
            return Json(result);
        }


        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}