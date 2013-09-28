using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using JustFood.Models;
using JustFood.Modules.Query;

namespace JustFood.Areas.Admin.Controllers {
    public class AccountBalanceController : Controller {
        private readonly JustFoodDBEntities _Db;
        private readonly UserInfo _Userinfo;

        public AccountBalanceController() {
            _Db = new JustFoodDBEntities();
            _Userinfo = new UserInfo();
        }

        public ActionResult Index() { return View(); }

        public ActionResult List() {
            List<ViewAccountBalance> accountbalances = _Db.ViewAccountBalances.ToList();
            DbSet<ViewSummaryAccountBalance> summary = _Db.ViewSummaryAccountBalances;
            double? sum = summary.Sum(n => n.Balance);
            ViewBag.Summary = summary;
            ViewBag.Sum = sum;
            var accounts = new AccountBalanceMultiple();
            accounts.ViewAccountBalance = accountbalances;
            return View(accounts);
        }

        public ActionResult Search() { return View("~/Views/Shared/EditorTemplates/AccountBalanceSearch.cshtml"); }

        [HttpPost]
        public ActionResult Search(AccountBalanceSearch search) {
            IEnumerable<ViewAccountBalance> accountbalances = _Db.ViewAccountBalances;
            if (search.Year != null && search.Year > 2000) {
                accountbalances = accountbalances.Where(n => n.Dated.Year == search.Year);
            }
            if (search.Month >= 1 && search.Month <= 12) {
                accountbalances = accountbalances.Where(n => n.Dated.Month == search.Month);
            }

            if (!string.IsNullOrWhiteSpace(search.ByUser)) {
                search.ByUser = search.ByUser.ToLower();
                accountbalances = accountbalances.Where(n => n.AccountOfLog.ToLower()
                                                              .Contains(search.ByUser) || n.AccountOfName.ToLower()
                                                                                           .Contains(search.ByUser));
            }
            DbSet<ViewSummaryAccountBalance> summary = _Db.ViewSummaryAccountBalances;
            double? sum = summary.Sum(n => n.Balance);
            ViewBag.Summary = summary;
            ViewBag.Sum = sum;
            var accounts = new AccountBalanceMultiple();
            accounts.ViewAccountBalance = accountbalances.ToList();
            return View("List", accounts);
        }

        public ActionResult CurrentMonth() {
            IEnumerable<ViewAccountBalance> accountbalances = _Db.ViewAccountBalances;
            accountbalances = accountbalances.Where(n => n.Dated.Year == DateTime.Now.Year && n.Dated.Month == DateTime.Now.Month);


            DbSet<ViewSummaryAccountBalance> summary = _Db.ViewSummaryAccountBalances;
            double? sum = summary.Sum(n => n.Balance);
            ViewBag.Summary = summary;
            ViewBag.Sum = sum;
            var accounts = new AccountBalanceMultiple();
            accounts.ViewAccountBalance = accountbalances.ToList();
            return View("List", accounts);
        }

        public ActionResult FilterBy(int id) {
            if (id > -1) {
                User user = _Db.Users.Find(id);
                if (user != null) {
                    IQueryable<ViewAccountBalance> accountbalances = _Db.ViewAccountBalances.Where(n => n.AccountOf == id);
                    ViewBag.Summary = null;
                    ViewBag.Sum = null;
                    ViewBag.Name = user.Name;
                    var accounts = new AccountBalanceMultiple();
                    accounts.ViewAccountBalance = accountbalances.ToList();
                    return View("List", accounts);
                }
            }
            return View("Error");
        }

        private void GetUsers() {
            ViewBag.AccountOf = new SelectList(_Db.Users.Where(n => n.IsOwner)
                                                  .ToList(), "UserID", "Name");
        }

        public ActionResult Create() {
            GetUsers();
            return View();
        }

        //
        // POST: /Admin/AccountBalance/Create

        [HttpPost]
        public ActionResult Create(AccountBalance accountbalance) {
            if (User.Identity.IsAuthenticated) {
                accountbalance.Dated = DateTime.UtcNow;
                accountbalance.AddBy = _Userinfo.GetUserSession()
                                                .UserID;
                accountbalance.IsBoughtProduct = false;
                accountbalance.IsAddedMoney = true;
                accountbalance.IsExpense = false;
                if (accountbalance.Amount < 0) {
                    ModelState.AddModelError("Amount", "Amount can't be negative.");
                }
                if (ModelState.IsValid) {
                    _Db.AccountBalances.Add(accountbalance);
                    _Db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            GetUsers();
            return View(accountbalance);
        }

        protected override void Dispose(bool disposing) {
            _Db.Dispose();
            base.Dispose(disposing);
        }
    }
}