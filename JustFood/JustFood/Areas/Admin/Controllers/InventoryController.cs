using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using JustFood.Models;
using JustFood.Modules.Query;
using System;
using JustFood.Modules.StaticContents;
using JustFood.Modules.Cookie;

namespace JustFood.Areas.Admin.Controllers {
    public class InventoryController : Controller {
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();
        private readonly UserInfo userinfo = new UserInfo();

        //
        // GET: /Admin/Inventory/

        public ActionResult Index() {
            List<Inventory> inventories = db.Inventories.Include(i => i.Category).Include(n=> n.QuantityType)
                                            .ToList();
            IQueryable<Category> categories = db.Categories.Where(n => !n.IsExpense);
            var qty = db.QuantityTypes.FirstOrDefault();
            if (qty == null) {
                return View("Error2", 
                    new Exception("No quantity type exist."));
            }
            foreach (Category category in categories) {
                if (!inventories.Any(n => n.CategoryID == category.CategoryID)) {
                    var inventory = new Inventory {
                        CategoryID = category.CategoryID,
                        Quantity = 0, 
                        LastEditedby = userinfo.GetUserID(),
                        QtyType = qty.QuantityTypeID
                    };
                    db.Inventories.Add(inventory);
                    inventories.Add(inventory);
                }
            }

            db.SaveChanges();
            return View(inventories);
        }

        public ActionResult Details(int id) {
            var inventoryview = new InventoryDetailsView(id);
            return View(inventoryview);
        }

        public ActionResult Edit(int id) {
            var inventory = db.Inventories.Find(id);
            return View(inventory);
        }

        [HttpPost]
        public ActionResult Edit(Inventory inventory) {
            var inventory2 = db.Inventories.Find(inventory.InventoryID);
            inventory2.SoldPrice = inventory.SoldPrice;
            //removing session information.
            Statics.Cookies.Remove(CookiesNames.InventoryEmpty);
            db.SaveChanges();
            return RedirectToActionPermanent("Index");
        }

        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}