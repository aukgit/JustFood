using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using JustFood.Modules.Extensions;
using JustFood.Modules.TimeZone;
using Microsoft.Ajax.Utilities;
using DB = JustFood.Models;
using JustFood.Modules.Query;
using JustFood.Modules.Session;
using JustFood.Modules.Cookie;
using JustFood.Modules.Cache;
using JustFood.Models;
using JustFood.Modules.StaticContents;
using JustFood.Modules.Message;
using JustFood.Areas.Admin.Controllers;

namespace JustFood.Areas.Sale.Controllers {
    public class HomeController : Controller {
        readonly DB.JustFoodDBEntities db;
        readonly UserInfo userinfo;
        Exception exceptionNoRights = new Exception("Sorry you have no rights to process this function. Please contact with respective admin for more details.");

        public HomeController() {
            db = new DB.JustFoodDBEntities();
            userinfo = new UserInfo();
        }

        #region Extention Methods and Propertise


        /// <summary>
        /// Check and save current time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Checks and returns if times are equal or not from the cookie and then save the cookie.</returns>
        bool IsCookieTimeEqual(string time) {
            string ctime = CookieTime;
            //save current time to cookie.
            CookieTime = time;
            if (ctime != null && time != null && time.Equals(ctime)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// checks null and return the value.
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        bool IsToday(DB.Sale sale) {
            if (sale != null) {
                if (sale.Date.Date == DateTime.Now.Date) {
                    return true;
                }
            }
            return false;
        }

        public String CookieTime {
            get {
                return Statics.Cookies.ReadString(CookiesNames.Time);
            }
            set {
                Statics.Cookies.Save(value, CookiesNames.Time);
            }
        }

        public string CookieIsInventoryEmpty {
            get {
                return Statics.Cookies.ReadString(CookiesNames.InventoryEmpty);
            }
            set {
                Statics.Cookies.Save(value, CookiesNames.InventoryEmpty);
            }
        }


        /// <summary>
        /// Keep only today's sale.
        /// </summary>
        public DB.Sale CacheSale {
            get { return (DB.Sale)Statics.Caches.Get(CacheNames.Sale); }
            set { Statics.Caches.Set(CacheNames.Sale, value); }
        }
        #endregion

        public ActionResult Index() {

            string date = Zone.GetDate(DateTime.Now);

            return View();
        }

        #region Searching

        DB.Sale GetToday() {
            DB.Sale sale;
            sale = CacheSale;
            if (sale != null && IsToday(sale)) {
                try {
                    db.Entry(sale).State = EntityState.Modified;
                } catch (Exception ex) { }
                return sale;
            }
            DateTime date = DateTime.Now.Date;
            sale = db.Sales.FirstOrDefault(n => n.Date == date);
            CacheSale = sale;
            return sale;
        }

        private DB.Sale GetDatedSale(DateTime date) {
            var dated = date.Date;
            DB.Sale sale = CacheSale;
            if (sale != null && IsToday(sale)) {
                try {
                    db.Entry(sale).State = EntityState.Modified;
                } catch (Exception ex) { }
                return sale;
            }
            sale = db.Sales.FirstOrDefault(n => n.Date == dated);
            CacheSale = sale;
            return sale;
        }

        /// <summary>
        /// Will be added with the context.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private DB.Sale GetDatedSale(int id) {
            DB.Sale sale = CacheSale;
            if (sale != null && IsToday(sale)) {
                try {
                    db.Entry(sale).State = EntityState.Modified;
                } catch (Exception ex) { }
                return sale;
            }
            sale = db.Sales.Find(id);
            CacheSale = sale;
            return sale;
        }

        Inventory GetInventory(int CategoryID) {
            return db.Inventories.FirstOrDefault(n => n.CategoryID == CategoryID);
        }
        #endregion

        #region Quantity Increase & Decrease
        void IncreaseSaleQuantity(DB.DetailedSale dSale, short number = 1) {
            dSale.Qty = number;
        }
        void DecreaseSaleQuantity(DB.DetailedSale dSale, short number = -1) {
            dSale.Qty = number;
        }

        #endregion

        #region Transaction Posibility : if user has permission to do the transaction at current period of time.

        /// <summary>
        /// Returning true means rejected.
        /// Do not execute the transaction.
        /// </summary>
        /// <param name="SaleID"></param>
        /// <returns></returns>
        bool IsTransactionRejected(int SaleID) {
            var user = userinfo.GetUserSession();
            if (user.IsAccessToAdmin) {
                return false;
            }

            var sale = (DB.Sale)Session["Sale"];

            if (sale == null) {
                sale = GetDatedSale(SaleID);
            }
            Session["Sale"] = sale;
            //if date is not valid to today or is already check then return false.
            if (sale.Date.Date != DateTime.Now.Date || sale.IsDiscardsChecked || sale.IsDividedAmongPartners) {
                return true;
            }
            return false;
        }

        bool IsTransactionRejected(DB.Sale sale) {
            var user = userinfo.GetUserSession();
            if (user.IsAccessToAdmin) {
                return false;
            }
            Session["Sale"] = sale;
            //if date is not valid to today or is already check then return true.
            if (sale.Date.Date != DateTime.Now.Date || sale.IsDiscardsChecked || sale.IsDividedAmongPartners) {
                return false;
            }
            return true;
        }

        #endregion
        
        #region First Step: Start Business or Create Daily Sale Ledger

        private int CreateSale(int inHandCash) {
            DB.Sale today = GetToday();
            if (today == null) {
                var sale = new DB.Sale {
                    Date = DateTime.Now,
                    TotalSold = 0,
                    IsDividedAmongPartners = false,
                    OtherExpenses = 0,
                    TotalAcheived = 0,
                    Discount = 0,
                    TotalLess = 0,
                    TotalWastages = 0,
                    TotalDiscountNumber = 0,
                    InHandCashChange = inHandCash,
                    ActualAcheivedFromEmployee = 0,
                    ActualLossProfitAfterCollection = 0,
                    IsInventorySet = false
                };
                db.Sales.Add(sale);
                db.SaveChanges();
                return sale.SaleID;
            } else {
                return today.SaleID;
            }
        }


        ActionResult CreateView() {
            var user = userinfo.GetUser();
            if (user != null && !user.IsAccessToAdmin) {
                return View("Error2", exceptionNoRights);
            }

            return View();
        }

        public ActionResult Create() {
            return CreateView();
        }

        /// <summary>
        /// Save only the cash and start the business.
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="Cash"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int? Cash) {
            if (Cash == null || Cash <= 0) {
                ModelState.AddModelError("Cash", "Cash can't be negative or zero.");
                return CreateView();
            }
            int saleid = CreateSale((int)Cash);
            db.SaveChanges();
            return RedirectToAction("Today");
        }



        #endregion

        #region Second Step: Setting Inventory & Daily Stock information

        [HttpGet]
        public ActionResult InventorySetter() {
            var sale = GetToday();
            if (sale == null) {
                return View("Error2", new Exception("Sale is not valid in the inventory setter. Contact with developer."));
            }

            //if (sale != null && sale.IsInventorySet) {
            //    return DetermineSaleDay(sale);
            //}

            List<DB.Inventory> invetories = db.Inventories
                                              .Include(c => c.Category)
                                              .ToList();
            var quantityNullStr = CookieIsInventoryEmpty;
            bool quantityNull = false;
            if (quantityNullStr == null) {
                quantityNull = invetories.All(n => n.Quantity == 0);
                CookieIsInventoryEmpty = quantityNull.ToString(); //saving it in cookie.
            }
            if (invetories.Count == 0 || quantityNull) {
                return View("Error2", new Exception("Inventory is not valid or inventory has no item. Please contact with your admin."));
            }

            ViewBag.SaleID = sale.SaleID;
            return View(invetories);
        }

        [HttpPost]
        public ActionResult InventorySetter(FormCollection forms) {
            int SaleID = int.Parse(Request["SaleID"]);
            foreach (string category in forms.AllKeys) {
                double qty = double.Parse(forms.GetValue(category)
                                               .AttemptedValue);
                int parse = 0;
                if (int.TryParse(category, out parse)) {
                    CreateDailyStock(SaleID, parse, qty); //add to db
                }
            }
            var sale = GetDatedSale(SaleID);
            sale.IsInventorySet = true;

            db.SaveChanges();
            return DetermineSaleDay(sale);
        }

        /// <summary>
        /// Create Inventory daily stock what they found on the store.
        /// </summary>
        /// <param name="saleID"></param>
        /// <param name="categoryId"></param>
        /// <param name="qty"></param>
        private void CreateDailyStock(int saleID, int categoryId, double qty) {
            var stock = new DB.DailyStock {
                CategoryID = categoryId,
                Quantity = qty,
                SaleID = saleID
            };
            db.DailyStocks.Add(stock);
        }

        #endregion

        #region Generate Drop Down list in the all section of forms

        private void GenerateDropDowns() {
            var categories = db.Categories.ToList();
            ViewBag.DiscountCategoryID = new SelectList(
                                    categories.Where(n => n.IsSalable)
                                    .OrderByDescending(n => n.CategoryID)
                                    .Select(n => new { CategoryID = n.CategoryID, Category = n.Category1 })
                                    .ToList(),
                                    "CategoryID",
                                    "Category");
            ViewBag.ExpenseCategoryID = new SelectList(
                                    categories.Where(n => n.IsExpense && n.Category1 != "Salary")
                                    .OrderByDescending(n => n.CategoryID)
                                    .Select(n => new { CategoryID = n.CategoryID, Category = n.Category1 })
                                    .ToList(),
                                    "CategoryID",
                                    "Category");
            ViewBag.CategoryID = new SelectList(
                                  categories.Where(n => !n.IsExpense && !n.IsSalable)
                                  .OrderByDescending(n => n.CategoryID)
                                  .Select(n => new { CategoryID = n.CategoryID, Category = n.Category1 })
                                  .ToList(),
                                  "CategoryID",
                                  "Category");
            ViewBag.QuantityCategories = new SelectList(
                                 db.QuantityTypes
                                 .OrderByDescending(n => n.QuantityTypeID)
                                 .ToList(),
                                 "QuantityTypeID",
                                 "QtyType");
        }

        #endregion

        #region Category Wise Selling Information

        /// <summary>
        /// Add category wise sold += numberOfitemSold wheatear exist or not .
        /// However it doesn't perform SaveChanges()
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="CategoryID"></param>
        /// <param name="numberOfitemSold"></param>
        void AddCategoryWiseSellingInformation(DB.DetailedSale dSale, int numberOfitemSold = 1) {
            var cat = db.CategoryWiseSolds
                        .FirstOrDefault(n => n.SaleID == dSale.SaleID && n.CategoryID == dSale.CategoryID);
            if (cat == null) {
                cat = new CategoryWiseSold() {
                    CategoryWiseSoldID = Guid.NewGuid(),
                    CategoryID = dSale.CategoryID,
                    SaleID = dSale.SaleID,
                    Quantity = 0
                };
            }
            if (cat != null) {
                cat.Quantity += numberOfitemSold;
            }
        }

        /// <summary>
        /// Remove category wise sold -= numberOfitemSold wheatear exist or not .
        /// However it doesn't perform SaveChanges()
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="CategoryID"></param>
        /// <param name="numberOfitemSold"></param>
        void RemoveCategoryWiseSellingInformation(DB.DetailedSale dSale, int numberOfitemSold = 1) {
            var cat = db.CategoryWiseSolds
                        .FirstOrDefault(n => n.SaleID == dSale.SaleID && n.CategoryID == dSale.CategoryID);
            if (cat == null) {
                cat = new CategoryWiseSold() {
                    CategoryWiseSoldID = Guid.NewGuid(),
                    CategoryID = dSale.CategoryID,
                    SaleID = dSale.SaleID,
                    Quantity = 0
                };
            }
            if (cat != null) {
                cat.Quantity -= numberOfitemSold;
            }
        }

        #endregion

        #region Details of individual Sales

        public ActionResult DetailsOf(int SaleID, int CategoryID, string time) {
            var details = db.DetailedSales
                            .Where(n => n.SaleID == SaleID &&
                                        n.CategoryID == CategoryID)
                            .Include(n => n.Category)
                            .Include(n => n.User)
                            .OrderByDescending(n => n.Time)
                            .ToList();
            if (details.Count == 0) {
                MessageSetter.Set("Sorry there is no record.");
                return DetermineSaleDay(SaleID);
            }

            ViewBag.TimeZone = Zone.Get();
            ViewBag.Cash = details.Sum(n => n.SoldAt);
            ViewBag.Items = details.Count(n => !n.IsMarkedDiscard);
            ViewBag.Discounts = details.Sum(n => n.Discount);
            ViewBag.Discards = details.Count(n => n.IsDiscard);
            return View(details);
        }

        #endregion

        #region Selling & Discount

        public ActionResult Discount() {
            return RedirectToActionPermanent("Today");
        }

        public ActionResult SaleItem(int SaleID, int CategoryID, string time) {

            #region Check Cache
            var saleCache = CacheSale;
            if (IsCookieTimeEqual(time)) {
                MessageSetter.SetWarning("Sorry , previous transaction is not saved to the database. Because same transaction is placed before.");

                if (saleCache != null) {
                    //if (IsToday(saleCache)) {
                    //    // today
                    //    return RedirectToActionPermanent("Today");
                    //} else {
                    return DetermineSaleDay(saleCache);
                    //}
                }
                return DetermineSaleDay(SaleID);
            }
            #endregion

            var sale = GetDatedSale(SaleID);
            if (sale == null) {
                return RedirectToActionPermanent("Today");
            }
            var inventory = db.Inventories.Include(n => n.Category).FirstOrDefault(n => n.CategoryID == CategoryID);

            if (IsTransactionRejected(sale)) {
                return View("Error2", new Exception("Sorry you have no rights to change previous sales information."));
            }

            if (sale != null && inventory != null) {
                var saleItem = new DB.DetailedSale() {
                    CategoryID = CategoryID,
                    IsDiscard = false,
                    SaleID = SaleID,
                    SellingPrice = (int)inventory.SoldPrice,
                    SoldAt = (int)inventory.SoldPrice,
                    Time = DateTime.Now,
                    IsMarkedDiscard = false,
                    UserID = userinfo.GetUserID()
                };
                //do some inventory out
                //not implemented yet...
                InventoryOut(sale, inventory);
                sale.TotalAcheived += saleItem.SoldAt;
                sale.TotalSold += saleItem.SoldAt;
                db.DetailedSales.Add(saleItem);

                //Increasing quantity of sale
                IncreaseSaleQuantity(saleItem);

                //add category wise sale
                AddCategoryWiseSellingInformation(saleItem);

                string categoryDisplay = inventory.Category.Category1;

                MessageSetter.SetPositive(categoryDisplay + " +1.");

                db.SaveChanges();
                CacheSale = sale;
            } else {
                throw new Exception("Sale or Inventory is not found.");
            }
            return DetermineSaleDay(sale);

        }

        [HttpPost]
        public ActionResult Discount(DiscountView discountView, string time) {
            if (ModelState.IsValid) {

                #region Check Cache

                int SaleID = discountView.SaleID;
                var saleCache = CacheSale;
                if (IsCookieTimeEqual(time)) {
                    MessageSetter.SetWarning("Sorry , previous transaction is not saved to the database. Because same transaction is placed before.");
                    if (saleCache != null) {
                        if (IsToday(saleCache)) {
                            // today
                            return RedirectToActionPermanent("Today");
                        } else {
                            return DetermineSaleDay(saleCache);
                        }
                    }
                    return DetermineSaleDay(SaleID);
                }
                #endregion

                var sale = GetDatedSale(SaleID);

                if (IsTransactionRejected(sale)) {
                    return View("Error2", new Exception("Sorry you have no rights to change previous sales information."));
                }

                var inventory = db.Inventories.Include(n => n.Category).FirstOrDefault(n => n.CategoryID == discountView.DiscountCategoryID);

                if (sale != null && inventory != null) {
                    int soldAt = discountView.Amount;
                    int discount = (int)inventory.SoldPrice - soldAt;

                    var saleItem = new DB.DetailedSale {
                        CategoryID = discountView.DiscountCategoryID,
                        IsDiscard = false,
                        SaleID = discountView.SaleID,
                        SellingPrice = (int)inventory.SoldPrice,
                        SoldAt = soldAt,
                        Discount = discount,
                        Time = DateTime.Now,
                        IsMarkedDiscard = false,
                        DiscountPerson = discountView.Name,
                        DiscountNumber = discountView.Phone,
                        Note = discountView.Notes,
                        UserID = userinfo.GetUserID()
                    };
                    // do some inventory out
                    // not implemented yet...
                    InventoryOut(sale, inventory);
                    sale.TotalAcheived += soldAt;
                    sale.TotalSold += soldAt;
                    sale.TotalDiscountNumber++;
                    if (sale.Discount == null)
                        sale.Discount = 0;
                    sale.Discount += discount;
                    db.DetailedSales.Add(saleItem);

                    //Increasing quantity of sale
                    IncreaseSaleQuantity(saleItem);

                    //add category wise sale
                    AddCategoryWiseSellingInformation(saleItem);

                    db.SaveChanges();
                    string categoryDisplay = inventory.Category.Category1;
                    MessageSetter.SetPositive(categoryDisplay + " +1 , discount @" + discount + ".");
                    CacheSale = sale;

                } else {
                    throw new Exception("Sale or Inventory is not found.");
                }
                return DetermineSaleDay(sale);

            }
            return View("Error");
        }


        #endregion

        #region Discard Sales

        public ActionResult DiscardItem(int SaleID, int CategoryID, string time) {

            #region Check Cache
            var saleCache = CacheSale;
            if (IsCookieTimeEqual(time)) {
                if (saleCache != null) {
                    if (IsToday(saleCache)) {
                        // today
                        return RedirectToActionPermanent("Today");
                    } else {
                        return DetermineSaleDay(saleCache);
                    }
                }
                return DetermineSaleDay(SaleID);
            }
            #endregion


            var sale = GetDatedSale(SaleID);
            if (sale == null) {
                return RedirectToActionPermanent("Today");
            }

            if (IsTransactionRejected(sale)) {
                return View("Error2", new Exception("Sorry you have no rights to change previous sales information."));
            }

            var inventory = db.Inventories.Include(n => n.Category).FirstOrDefault(n => n.CategoryID == CategoryID);
            var lastSold = db.DetailedSales.OrderByDescending(n => n.Time)
                             .FirstOrDefault(n => n.SaleID == SaleID && n.CategoryID == CategoryID && !n.IsDiscard && !n.IsMarkedDiscard);
            var existRecord = sale != null && inventory != null;

            var saleView = new SaleDayView(SaleID) {
                Sale = sale
            };
            ///means item still sold and can be discarded
            bool soldOut = false;
            var summary = saleView.GetSummary();
            var specificCategory = summary.FirstOrDefault(n => n.SaleID == SaleID && n.CategoryID == CategoryID);
            if (specificCategory != null) {
                // if any summary of that item exist.
                // if any item is still sold from this category.
                soldOut = !(specificCategory.SoldQuantity > 0); //if still one sold then it will be false.
            }

            if (existRecord && !soldOut) {

                //first check if one product is sold or not.
                //if not then can't discard.

                var saleItem = new DB.DetailedSale() {
                    CategoryID = CategoryID,
                    SaleID = SaleID,
                    SellingPrice = (int)inventory.SoldPrice,
                    SoldAt = lastSold.SoldAt * -1,
                    Time = DateTime.Now,
                    IsDiscard = true,
                    UserID = userinfo.GetUserID()

                };

                // do some inventory in
                // not implemented yet...
                InventoryIn(sale, inventory);
                sale.TotalAcheived += saleItem.SoldAt; //because sold at is -
                sale.TotalSold += saleItem.SoldAt;

                //fixing discount money
                if (lastSold.Discount != null && lastSold.Discount > 0) {
                    sale.Discount -= lastSold.Discount;
                }
                //tracking discard sales
                sale.DiscardSales++;

                if (sale.DiscardSales > 20) {
                    sale.AnyProblem = true;
                }

                lastSold.IsMarkedDiscard = true;//when discarded, marked it.
                db.DetailedSales.Add(saleItem);
                string categoryDisplay = inventory.Category.Category1;
                MessageSetter.SetPositive(categoryDisplay + " -1.");

                //decreasing quantity of sale
                DecreaseSaleQuantity(saleItem);

                //remove category wise sale
                RemoveCategoryWiseSellingInformation(saleItem);

                db.SaveChanges();
                CacheSale = sale;
                return DetermineSaleDay(sale);
            } else if (existRecord && soldOut) {
                // do nothing , send to the view.
                if (IsToday(sale)) {
                    return RedirectToActionPermanent("Today");
                }
                return DetermineSaleDay(sale);
            } else {
                throw new Exception("Information about discarding not found.");
            }
        }

        #endregion

        #region Determining the day

        private ActionResult DetermineSaleDay(DB.Sale sale, DB.SaleDayView saleView) {
            if (sale != null) {
                if (!sale.IsInventorySet) {
                    return RedirectToActionPermanent("InventorySetter");
                }
                if (saleView == null) {
                    saleView = new DB.SaleDayView(sale.SaleID) {
                        Sale = sale,
                        SaleDated = sale.Date
                    };
                }
                saleView.DiscountView = new DB.DiscountView {
                    SaleID = saleView.SaleID
                };

                ViewBag.SaleID = sale.SaleID;
                GenerateDropDowns();
                return View("Day", saleView);
            }
            var ex = new Exception("Sale is invalid.");
            return View("Error2", ex);
        }

        private ActionResult DetermineSaleDay(int saleId) {
            var sale = GetDatedSale(saleId);
            return DetermineSaleDay(sale);
        }

        private ActionResult DetermineSaleDay(DB.Sale sale) {
            return DetermineSaleDay(sale, null);
        }


        public ActionResult Today() {
            DB.Sale today = GetToday();
            if (today == null) {
                //create date.
                return RedirectToAction("Create");
            } else if (!today.IsInventorySet) {
                return RedirectToActionPermanent("InventorySetter");
            }
            return DetermineSaleDay(today);
        }


        #endregion

        #region Inventory In and Out information

        private void InventoryIn(DB.Sale sale, DB.Inventory inventory) {
        }

        void InventoryOut(DB.Sale sale, DB.Inventory inventory) {

        }

        #endregion

        #region Requsion

        public ActionResult Requisition(Requisition requisition, string time, int SaleIDReq) {
            #region Check Cache

            var saleCache = CacheSale;
            if (IsCookieTimeEqual(time)) {
                MessageSetter.SetWarning("Sorry , previous transaction is not saved to the database.");
                if (saleCache != null) {
                    if (IsToday(saleCache)) {
                        // today
                        return RedirectToActionPermanent("Today");
                    } else {
                        return DetermineSaleDay(saleCache);
                    }
                }
            }
            #endregion

            var req = new Requisition() {
                AddedBy = userinfo.GetUserID(),
                QtyType = requisition.QtyType,
                Quantity = requisition.Quantity,
                CategoryID = requisition.CategoryID,
                IsAutoAdded = false,
                IsInProcess = false,
                IsDone = false
            };
            var categoryFound = db.Categories.FirstOrDefault(n => n.CategoryID == requisition.CategoryID);
            string quantityType = "";
            if (categoryFound != null) {
                var category = categoryFound.Category1;
                var quantityTypeObject = db.QuantityTypes.FirstOrDefault(n => n.QuantityTypeID == requisition.QtyType);
                if (quantityTypeObject != null) {
                    quantityType = quantityTypeObject.QtyType;
                } else {
                    MessageSetter.SetError("Requisition failed " + category);
                    goto skip;
                }

                db.Requisitions.Add(req);
                db.SaveChanges();
                MessageSetter.SetPositive("Requisition successfully " + category + " " + req.Quantity + " " + quantityType + " added.");
            } else {
                MessageSetter.SetError("Requisition failed.");
            }

        skip:
            return DetermineSaleDay(SaleIDReq);
        }
        
        #endregion

        #region Expense

        public ActionResult Expense(ExpenseSalesView expense, string time) {

            #region Check Cache

            var saleCache = CacheSale;
            if (IsCookieTimeEqual(time)) {
                MessageSetter.SetWarning("Sorry , previous transaction is not saved to the database.");
                if (saleCache != null) {
                    if (IsToday(saleCache)) {
                        // today
                        return RedirectToActionPermanent("Today");
                    } else {
                        return DetermineSaleDay(saleCache);
                    }
                }
            }
            #endregion


            int SaleID = expense.SaleID;
            const string dailyExpense = "Daily Expense";
            
            if (expense.Amount < 0) {
                ModelState.AddModelError("Amount", "Amount can't be negative.");
                return DetermineSaleDay(SaleID);
            }
            SaleDayView day = new SaleDayView(SaleID);
            var userAccExtention = new UserAccountsExtension();
            var accountOf = userAccExtention.GetExpenseAccount();
            var accountBalance = new AccountBalance() {
                AccountOf = accountOf.UserID,
                AddBy = userinfo.GetUserID(),
                Amount = expense.Amount * (-1),
                Dated = DateTime.Now,
                CategoryProduct = expense.ExpenseCategoryID,
                Note = expense.Note,
                IsExpense = true,
                IsAddedMoney = false,
                IsBoughtProduct = false,
                IsVerified = false
            };
            db.AccountBalances.Add(accountBalance);
            MessageSetter.SetPositive("Expense of " + expense.Amount + " added successfully.");
            db.SaveChanges();
            return DetermineSaleDay(SaleID);
        }
        
        #endregion

        #region Add Product
        
        public ActionResult AddProduct(AddProductView addProduct, string time, int SaleID) {
            #region Check Cache

            var saleCache = CacheSale;
            if (IsCookieTimeEqual(time)) {
                MessageSetter.SetWarning("Sorry , previous transaction is not saved to the database.");
                if (saleCache != null) {
                    if (IsToday(saleCache)) {
                        // today
                        return RedirectToActionPermanent("Today");
                    } else {
                        return DetermineSaleDay(saleCache);
                    }
                }
            }
            #endregion
            
            if (addProduct.Cost < 0) {
                ModelState.AddModelError("Amount", "Amount can't be negative.");
                return DetermineSaleDay(SaleID);
            }

            var useraccExt = new UserAccountsExtension();
            var accountOf = useraccExt.GetProductBoughtUser();

            
            var accountBalance = new AccountBalance() {
                AccountOf = accountOf.UserID,
                AddBy = userinfo.GetUserID(),
                Amount = addProduct.Cost,
                AddedQuantity = addProduct.Quantity,
                QtyType = addProduct.QuantityType,
                Dated = DateTime.Now,
                CategoryProduct = addProduct.CategoryID,
                IsExpense = true,
                IsAddedMoney = false,
                IsBoughtProduct = true,
                IsVerified = false
            };

            var inventoryEntension = new InventoryExtension();
            if (inventoryEntension.InventoryAdd(db, accountBalance)) {
                db.SaveChanges();
                MessageSetter.SetPositive("Product of " + addProduct.Cost + " successfully.");
            } else {
                MessageSetter.SetWarning("try again.");

            }
            return DetermineSaleDay(SaleID);
        }
        
        #endregion
        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}