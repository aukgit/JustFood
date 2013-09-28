using System;
using System.Linq;
using System.Web.Mvc;
using JustFood.Models;
using JustFood.Modules.Extensions;
using JustFood.Modules.Query;

namespace JustFood.Areas.Admin.Controllers {
    public class ProductsController : Controller {
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();
        private readonly UserInfo userinfo = new UserInfo();

        private void GetUsers() {
            ViewBag.AccountOf = new SelectList(db.Users.Select(n => new {n.UserID, n.Name})
                                                 .ToList(), "UserID", "Name");
        }

        private void GetCategories() { ViewBag.CategoryProduct = new SelectList(db.Categories, "CategoryID", "Category1"); }

        void GetDropDowns() {
            GetCategories();
            GetUsers();

            ViewBag.QtyType = new SelectList(db.QuantityTypes.ToList(), "QuantityTypeID", "QtyType");
        }

        public ActionResult Add() {
            GetDropDowns();
            return View();
        }

    

        [HttpPost]
        public ActionResult Add(AccountBalance accountbalance) {
            if (User.Identity.IsAuthenticated) {
                double? sum = db.ViewSummaryAccountBalances.Sum(n => n.Balance);
                if (sum == null) {
                    sum = 0;
                }
                var sum2 = (double) sum;

                if (sum2 < accountbalance.Amount) {
                    ModelState.AddModelError("Amount", "Sorry your amount exceeds your account balance " + sum2 + ".");
                }
                var inventoryExtension = new InventoryExtension();
                if (ModelState.IsValid && inventoryExtension.InventoryAdd(db,accountbalance)) {
                    db.SaveChanges();
                    return RedirectToAction("List", "AccountBalance");
                }
            }
            GetDropDowns();

            return View(accountbalance);
        }


        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}