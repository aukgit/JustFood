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
    public class QtyConverterController : Controller
    {
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();

        //
        // GET: /Admin/QtyConverter/

        public ActionResult Index()
        {
            var quantityconversations = db.QuantityConversations.Include(q => q.QuantityType);
            return View(quantityconversations.ToList());
        }

        void GetDropDowns(QuantityConversation quantityconversation) {
            var qtyTypes = db.QuantityTypes.ToList();
            ViewBag.QuantityTypeID = new SelectList(qtyTypes, "QuantityTypeID", "QtyType", quantityconversation.QuantityTypeID);
            ViewBag.ConvertedTypeID = new SelectList(qtyTypes, "QuantityTypeID", "QtyType", quantityconversation.ConvertedTypeID);
        }

        void GetDropDowns() {
            var qtyTypes = db.QuantityTypes.ToList();

            ViewBag.QuantityTypeID = new SelectList(qtyTypes, "QuantityTypeID", "QtyType");
            ViewBag.ConvertedTypeID = new SelectList(qtyTypes, "QuantityTypeID", "QtyType");
        }

  



        public ActionResult Create()
        {
            GetDropDowns();
            return View();
        }



        [HttpPost]
        public ActionResult Create(QuantityConversation quantityconversation)
        {
            if (ModelState.IsValid)
            {
                db.QuantityConversations.Add(quantityconversation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GetDropDowns(quantityconversation);
            return View(quantityconversation);
        }

        //
        // GET: /Admin/QtyConverter/Edit/5

        public ActionResult Edit(byte id = 0)
        {
            QuantityConversation quantityconversation = db.QuantityConversations.Find(id);
            if (quantityconversation == null)
            {
                return HttpNotFound();
            }
            GetDropDowns(quantityconversation);
            return View(quantityconversation);
        }

        //
        // POST: /Admin/QtyConverter/Edit/5

        [HttpPost]
        public ActionResult Edit(QuantityConversation quantityconversation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quantityconversation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GetDropDowns(quantityconversation);
            return View(quantityconversation);
        }

        //
        // GET: /Admin/QtyConverter/Delete/5

        public ActionResult Delete(byte id = 0)
        {
            QuantityConversation quantityconversation = db.QuantityConversations.Find(id);
            if (quantityconversation == null)
            {
                return HttpNotFound();
            }
            return View(quantityconversation);
        }

        //
        // POST: /Admin/QtyConverter/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(byte id)
        {
            QuantityConversation quantityconversation = db.QuantityConversations.Find(id);
            db.QuantityConversations.Remove(quantityconversation);
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