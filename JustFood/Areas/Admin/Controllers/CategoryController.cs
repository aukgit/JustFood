using System.Data;
using System.Linq;
using System.Web.Mvc;
using JustFood.Models;
using System.Data.Entity;
namespace JustFood.Areas.Admin.Controllers {
    public class CategoryController : Controller {
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();

        void GetDropDowns(Category category = null) {
            if (category != null) {
                ViewBag.QtyType = new SelectList(db.QuantityTypes.ToList(), "QuantityTypeID", "QtyType",category.QtyType);
                
            } else {
                ViewBag.QtyType = new SelectList(db.QuantityTypes.ToList(), "QuantityTypeID", "QtyType");
            }
        }

        public ActionResult Index() {
            return View(db.Categories.Include(n => n.QuantityType).ToList());
        }

        public ActionResult Create() {
            GetDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category) {
            if (ModelState.IsValid) {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GetDropDowns(category);
            return View(category);
        }

        //
        // GET: /Admin/Category/Edit/5

        public ActionResult Edit(int id = 0) {
            Category category = db.Categories.Find(id);
            if (category == null) {
                return HttpNotFound();
            }
            GetDropDowns(category);
            return View(category);
        }

        //
        // POST: /Admin/Category/Edit/5

        [HttpPost]
        public ActionResult Edit(Category category) {
            if (ModelState.IsValid) {
                db.Entry(category)
                  .State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GetDropDowns(category);
            return View(category);
        }

        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}