using System;
using System.Linq;
using System.Web.Mvc;
using JustFood.Models;
using JustFood.Modules.Query;

namespace JustFood.Areas.Admin.Controllers {
    public class ExpenseController : Controller {
        private readonly UserInfo _Userinfo = new UserInfo();
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();

        private void GetDropdowns() {
            ViewBag.CategoryProduct = new SelectList(db.Categories.Where(n => n.IsExpense)
                                                       .ToList(), "CategoryID", "Category1");
            ViewBag.AccountOf = new SelectList(db.Users, "UserID", "Name");
        }

        public ActionResult Create() {
            GetDropdowns();
            return View();
        }

        public void CreateExpense(AccountBalance accountbalance) {
            accountbalance.Dated = DateTime.Now;
            accountbalance.AddBy = _Userinfo.GetUserID();
            accountbalance.IsAddedMoney = false;
            accountbalance.IsExpense = true;
            accountbalance.IsBoughtProduct = false;

            if (accountbalance.Amount < 0) {
                ModelState.AddModelError("Amount", "Amount can't be negative.");
            }
            double? sum = db.ViewSummaryAccountBalances.Sum(n => n.Balance);
            if (sum == null) {
                sum = 0;
            }
            var sum2 = (double) sum;
            if (sum2 < accountbalance.Amount) {
                ModelState.AddModelError("Amount", "Sorry your amount excceds your account balance " + sum2 + ".");
            }
            accountbalance.Amount = accountbalance.Amount*-1;

            if (ModelState.IsValid) {
                db.AccountBalances.Add(accountbalance);
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult Create(AccountBalance accountbalance) {
            if (User.Identity.IsAuthenticated) {
                if (ModelState.IsValid) {
                    CreateExpense(accountbalance);
                    return RedirectToAction("List", "AccountBalance");
                }
            }
            GetDropdowns();
            return View(accountbalance);
        }

        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}