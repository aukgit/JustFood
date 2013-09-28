using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using JustFood.Models;
using JustFood.Modules.Query;
using JustFood.Modules.Role;

namespace JustFood.Controllers {
    [Authorize]
    public class AccountController : Controller {
        //
        // GET: /Account/Index

        public ActionResult Index() { return View(); }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login() { return View(); }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl) {
            if (ModelState.IsValid) {
                if (Membership.ValidateUser(model.UserName, model.Password)) {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && User.Identity.IsAuthenticated) {
                        //save last login
                        var userinfo = new UserInfo();
                        User user = userinfo.GetUserSession();
                        user.LastLogIn = DateTime.UtcNow;
                        using (var db = new JustFoodDBEntities()) {
                            db.Entry(user).State = EntityState.Detached;
                            db.Entry(user).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        return Redirect(returnUrl);
                    } else {
                        return RedirectToAction("Index", "Home");
                    }
                } else {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff() {
            FormsAuthentication.SignOut();
            HttpContext.Response.Cookies.Clear();
            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register() { return View(); }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model) {
            
            if (ModelState.IsValid) {
                //check the code
                var db = new JustFoodDBEntities();
                Code code = db.Codes.FirstOrDefault(m => m.Code1 == model.Code);
                if (code == null) {
                    ModelState.AddModelError(model.Code, "Your given code is not valid.");
                    return View(model);
                }


                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

                if (createStatus == MembershipCreateStatus.Success) {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);

                    //create new user.
                    if (code.Percentage == null) {
                        code.Percentage = 0;
                    }
                    if (code.Salary == null) {
                        code.Salary = 0;
                    }

                    var user = new User {
                        LogName = model.UserName,
                        Name = model.PersonName,
                        IsEmployee = code.IsEmployee,
                        IsOwner = code.IsOwner,
                        IsAccessToAdmin = code.IsAccessToAdmin,
                        Percentage = (double)code.Percentage,
                        Salary = (double)code.Salary,
                        Email = model.Email,
                        IsValidEmail = true
                    };
                    //there is no need to keep the used code.
                    db.Entry(code).State = EntityState.Deleted;
                    db.Users.Add(user);
                    db.SaveChanges();
                    Roles.CreateRole("");
                    var roleManager = new RoleManage();
                    string role = "";
                    if (user.IsAccessToAdmin) {
                        role = RoleNames.Admin;
                    } else if (user.IsEmployee) {
                        role = RoleNames.SalesMan;
                    }
                    roleManager.AddRole(user.LogName, role);
                    return RedirectToAction("Index", "Home");
                } else {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
       

        public ActionResult ChangePassword() { return View(); }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model) {
            if (ModelState.IsValid) {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                } catch (Exception) {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded) {
                    return RedirectToAction("ChangePasswordSuccess");
                } else {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess() { return View(); }

        public ActionResult ForgotPassword() { return View(); }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel forgotPasswordModel) {
            if (ModelState.IsValid) {
                return ForgetPasswordSuccess();
            }

            return View(forgotPasswordModel);
        }

        public ActionResult ForgetPasswordSuccess() { return View(); }

        #region Status Codes

        private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus) {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}