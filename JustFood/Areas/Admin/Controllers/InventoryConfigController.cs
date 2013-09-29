using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustFood.Models;

namespace JustFood.Areas.Admin.Controllers {
    public class InventoryConfigController : Controller {
        readonly JustFoodDBEntities db = new JustFoodDBEntities();


        public ActionResult Index() {
            var inventorySalables = db.ViewInventorySalables.ToList();
            ViewBag.InventoryOut = db.InventoryOutConfigs.Include(n => n.Category)
                                     .Include(n => n.Category1).OrderByDescending(n => n.InventoryOutConfigID).ToList();
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.QuantityTypes = db.QuantityTypes.ToList();
            return View(inventorySalables);
        }

        public void Add(int inventoryId) {
            var inventory = db.Inventories.Find(inventoryId);
            if(inventory!=null){
                var deductCategory = db.Categories.FirstOrDefault(n=> n.CategoryID != inventory.CategoryID);
                var inventoryOutConfig = new InventoryOutConfig() {
                    CategoryID = inventory.CategoryID, //category of that inventory
                    DiscardItemCategory = deductCategory.CategoryID, //discarding category when item is sold. For say burguer category needs to deduct from breads.
                    QtyType = deductCategory.QtyType,
                    PerSaleQuantity = 0
                };
                db.InventoryOutConfigs.Add(inventoryOutConfig);
                db.SaveChanges();

            }


           

        }

        public void Save(int? InventoryOutConfigID, int? CategoryID, int? DiscardCategoryID, decimal? Quantity, byte ?QuantityTypeID) {
            //var inventoryOutConfig = db.InventoryOutConfigs.Find(InventoryOutConfigID);

            //if (inventoryOutConfig != null) {
            //    inventoryOutConfig.CategoryID = CategoryID;
            //    inventoryOutConfig.DiscardItemCategory = DiscardCategoryID;
            //    inventoryOutConfig.QtyType = QuantityTypeID;
            //    inventoryOutConfig.PerSaleQuantity = Quantity;

            //    db.SaveChanges();
            //} else {
            //    throw new Exception("Record not found.");
            //}

        }


        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}