using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using JustFood.Models;
using JustFood.Modules.Role;

namespace JustFood.Areas.Admin.Controllers {
    public class UsersController : Controller {
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();

        //
        // GET: /Admin/Users/

        public ActionResult Index() {
            DbSet<User> users = db.Users;
            return View(users.ToList());
        }


        public ActionResult Edit(int id = 0) {
            User user = db.Users.Find(id);
            if (user == null) {
                return HttpNotFound();
            }
            ViewBag.Code = new SelectList(db.Codes, "CodeID", "Code1");
            return View(user);
        }

        //
        // POST: /Admin/Users/Edit/5

        [HttpPost]
        public ActionResult Edit(User user) {
            User prevUser = db.Users.Find(user.UserID);
            if (ModelState.IsValid) {
                var roleManage = new RoleManage();

                try {
                    //admin
                    roleManage.VerifyAddRemoveRole(user.LogName, RoleNames.Admin, prevUser.IsAccessToAdmin, user.IsAccessToAdmin);
                    //sales
                    roleManage.VerifyAddRemoveRole(user.LogName, RoleNames.Admin, prevUser.IsEmployee, user.IsEmployee);
                } catch (Exception ex) {
                    
                }
                db.Entry(prevUser)
                  .State = EntityState.Detached;
                db.Entry(user)
                  .State = EntityState.Modified;
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            return View(user);
        }

        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}