using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using JustFood.Models;
using JustFood.Modules.Cookie;
using JustFood.Modules.Session;
using AspUser = System.Web.Providers.Entities;

namespace JustFood.Modules.Query {
    public class UserInfo {
        JustFoodDBEntities _Db;
        MembershipUser _AspUser;
        User _DbUser;

        public UserInfo() {
            _Db = new JustFoodDBEntities();
        }

        public bool IsUserExist(string log) {
            if (_Db.Users.Any(n => n.LogName == log)) {
                return true;
            }
            return false;
        }

        public bool IsUserExist(int id) {
            if (_Db.Users.Any(n => n.UserID == id)) {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Returns custom database user.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public User GetUser(int userid) {
            if (_DbUser != null && _DbUser.UserID == userid) {
                return _DbUser;
            }
            return _DbUser = _Db.Users.Find(userid);
        }

        /// <summary>
        ///     Get current logged user record from db.
        /// </summary>
        /// <returns></returns>
        public User GetUser() {
            if (IsAuthenticated()) {
                return GetUser(GetAspUserCurrentUser().Identity.Name);
            }
            return null;
        }

        /// <summary>
        ///     Returns the custom database user.
        /// </summary>
        /// <param name="log">By log name.</param>
        /// <returns></returns>
        public User GetUser(string log) {
            if (_DbUser != null && _DbUser.LogName == log) {
                return _DbUser;
            }
            _DbUser = _Db.Users.FirstOrDefault(c => c.LogName == log);
            return _DbUser;
        }
        /// <summary>
        /// Returns all admin users from the database.
        /// </summary>
        /// <returns></returns>
        public IQueryable<User> GetAdmins() { return _Db.Users.Where(c => c.IsAccessToAdmin); }

        public DateTime? LastActive(string log) {
            if (_DbUser != null && _DbUser.LogName == log) {
                return _DbUser.LastLogIn;
            }

            _DbUser = GetUser(log);
            if (_DbUser != null) {
                return _DbUser.LastLogIn;
            }
            return null;
        }

        public bool IsAuthenticated() {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

       
        public bool IsCurrentUserSessionExist() {
            if (IsAuthenticated() && HttpContext.Current.Session != null && HttpContext.Current.Session[SessionNames.User] != null) {
                var user = (User)HttpContext.Current.Session[SessionNames.User];
                if (user == null) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        ///     If db user exist on the session return from session
        ///     or else get it and then save it to the session.
        /// </summary>
        /// <returns></returns>
        public User GetUserSession() {
            User user;
            if (IsCurrentUserSessionExist()) {
                user = (User) HttpContext.Current.Session[SessionNames.User];
                if (user != null) {
                    return user;
                }
            }
            user = GetUser(GetAspUserCurrentUser()
                               .Identity.Name);
            if (user == null) {
                return null;
            }
            HttpContext.Current.Session[SessionNames.User] = user;
            return user;
        }

        /// <summary>
        ///     Get user id from session or asp.net->db->session(keep);
        /// </summary>
        /// <returns>Return -1 when not found.</returns>
        public int GetUserID() {
            var useridCookie = HttpContext.Current.Request.Cookies[CookiesNames.UserID];

            if (useridCookie != null) {
                int userid;
                if (int.TryParse(useridCookie.Value, out userid)) {
                    return userid;
                }
            }
            User user = GetUserSession();
            if (user != null) {
                var cookieUser = new HttpCookie(Cookie.CookiesNames.UserID);
                cookieUser.Value = user.UserID.ToString();
                cookieUser.Expires = DateTime.Now.AddDays(60);
                HttpContext.Current.Response.Cookies.Set(cookieUser);
                return user.UserID;
            }
            return -1;
        }


        public string AuthenticatedUserName() {
            if (IsAuthenticated()) {
                return GetAspUserCurrentUser()
                    .Identity.Name;
            }
            return "";
        }

        public bool IsAuthenticated(string log) {
            if (_AspUser != null && log == _AspUser.UserName) {
                return _AspUser.IsOnline;
            }
            _AspUser = Membership.GetUser(log);
            return _AspUser != null && _AspUser.IsOnline;
        }

        public MembershipUser GetAspUser(string log) {
            if (_AspUser != null && log == _AspUser.UserName) {
                return _AspUser;
            }
            _AspUser = Membership.GetUser(log);
            return _AspUser;
        }

        public IPrincipal GetAspUserCurrentUser() {
            if (HttpContext.Current.User.Identity.IsAuthenticated) {
                return HttpContext.Current.User;
            }

            return null;
        }
    }
}