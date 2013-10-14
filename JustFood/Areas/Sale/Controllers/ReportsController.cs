using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustFood.Models;
using JustFood.Modules.StaticContents;
using JustFood.Modules.TimeZone;

namespace JustFood.Areas.Sale.Controllers
{
	public class ReportsController : Controller
	{
        private JustFoodDBEntities db = new JustFoodDBEntities();

        public ActionResult Index()
		{
            var sales = db.Sales.Where(n => !n.IsDividedAmongPartners || !n.IsDiscardsChecked).ToList();
            ViewBag.TimeZone = Zone.Get();
            return View(sales);
		}

	}
}
