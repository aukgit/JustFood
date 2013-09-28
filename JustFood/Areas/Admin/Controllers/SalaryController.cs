using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using JustFood.Models;
using JustFood.Modules.Query;

namespace JustFood.Areas.Admin.Controllers {
    public class SalaryController : Controller {
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();

        //
        // GET: /Admin/Salary/

        public ActionResult Index() {
            IQueryable<SalaryPaid> salarypaids = db.SalaryPaids.Include(s => s.User);
            return View(salarypaids.ToList());
        }

        public ActionResult Pay() {
            ViewBag.UserID = new SelectList(db.Users.Where(n => n.IsEmployee)
                                              .ToList(), "UserID", "LogName");
            return View();
        }

        //
        // POST: /Admin/Salary/Create

        [HttpPost]
        public ActionResult Pay(SalaryPaid salarypaid) {
            var userinfo = new UserInfo();
            User user = userinfo.GetUser(salarypaid.UserID);
            if (user != null) {
                salarypaid.PaidDate = DateTime.UtcNow;
                salarypaid.Salary = user.Salary;

                if (ModelState.IsValid) {
                    db.SalaryPaids.Add(salarypaid);
                    // add expense
                    Category categorySalary = db.Categories.FirstOrDefault(n => n.Category1 == "Salary");
                    if (categorySalary != null) {
                        var expenseController = new ExpenseController();
                        var accountBalance = new AccountBalance {AccountOf = salarypaid.UserID, Amount = salarypaid.Paid, CategoryProduct = categorySalary.CategoryID};
                        expenseController.CreateExpense(accountBalance);

                        db.SaveChanges();
                    } else {
                        goto Error;
                    }
                    return RedirectToAction("Index");
                }
            }
            Error:
            ViewBag.UserID = new SelectList(db.Users, "UserID", "LogName", salarypaid.UserID);
            return View(salarypaid);
        }

        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}