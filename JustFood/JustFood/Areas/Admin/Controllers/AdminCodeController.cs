using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using JustFood.Models;

namespace JustFood.Areas.Admin.Controllers {
    public class AdminCodeController : Controller {
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();

        public ActionResult Index() {
            List<Code> codes = db.Codes.Where(n => !n.IsUsed)
                                 .ToList();
            return View(codes);
        }

        public ActionResult Create() { return View(); }

        [HttpPost]
        public ActionResult Create(Code code) {
            try {
                code.GuidCode = Guid.NewGuid();

                if (ModelState.IsValid) {
                    db.Codes.Add(code);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            } catch {
                return View();
            }
        }

   

        //public ActionResult Edit(int id) {
        //    Code coderec = db.Codes.Find(id);
        //    if (coderec != null) {
        //        return View(coderec);
        //    }

        //    return View("Error");
        //}

        ////
        //// POST: /Admin/AdminCode/Edit/5

        //[HttpPost]
        //public ActionResult Edit(Code code) {
        //    try {
        //        if (ModelState.IsValid) {
        //            db.Entry(code)
        //              .State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //        return RedirectToAction("Index");
        //    } catch {
        //        return View();
        //    }
        //}

        public ActionResult Delete(int id) {
            Code found = db.Codes.Find(id);
            if (found != null) {
                db.Entry(found)
                  .State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Error");
        }

        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }

        //
        // GET: /Admin/AdminCode/Delete/5
    }
}