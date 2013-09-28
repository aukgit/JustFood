using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustFood.Models;

namespace JustFood.Areas.Admin.Controllers
{
    public class QtyController : Controller
    {
        private JustFoodDBEntities db = new JustFoodDBEntities();

        //
        // GET: /Admin/Qty/

        public ActionResult Index()
        {
            return View(db.QuantityTypes.ToList());
        }

        //
        // GET: /Admin/Qty/Details/5

        public ActionResult Details(byte id = 0)
        {
            QuantityType quantitytype = db.QuantityTypes.Find(id);
            if (quantitytype == null)
            {
                return HttpNotFound();
            }
            return View(quantitytype);
        }

        //
        // GET: /Admin/Qty/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Qty/Create

        [HttpPost]
        public ActionResult Create(QuantityType quantitytype)
        {
            if (ModelState.IsValid)
            {
                db.QuantityTypes.Add(quantitytype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(quantitytype);
        }

        //
        // GET: /Admin/Qty/Edit/5

        public ActionResult Edit(byte id = 0)
        {
            QuantityType quantitytype = db.QuantityTypes.Find(id);
            if (quantitytype == null)
            {
                return HttpNotFound();
            }
            return View(quantitytype);
        }

        //
        // POST: /Admin/Qty/Edit/5

        [HttpPost]
        public ActionResult Edit(QuantityType quantitytype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quantitytype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quantitytype);
        }

        //
        // GET: /Admin/Qty/Delete/5

        public ActionResult Delete(byte id = 0)
        {
            QuantityType quantitytype = db.QuantityTypes.Find(id);
            if (quantitytype == null)
            {
                return HttpNotFound();
            }
            return View(quantitytype);
        }

        //
        // POST: /Admin/Qty/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(byte id)
        {
            QuantityType quantitytype = db.QuantityTypes.Find(id);
            db.QuantityTypes.Remove(quantitytype);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}