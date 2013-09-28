using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustFood.Modules.ObjectToArray;
using JustFood.Modules.Type;

namespace JustFood.Modules.Cookie {
    /// <summary>
    /// Set cookies in response 
    /// Retrieve cookies from request.
    /// </summary>
    public class CookieProcessor {

        ControllerContext controllerContext;

        readonly string CookieName = "";

        DateTime defaultExpiration = DateTime.Now.AddHours(5);

        #region Constructor

        //public CookieProcessor(ControllerContext context) {
        //    this.controllerContext = context;
        //    this.httpContext = this.controllerContext.HttpContext;
        //}

        /// <summary>
        /// 
        /// </summary>
        public CookieProcessor() {
        }

        /// <summary>
        /// Pass Base.ControllerContext
        /// </summary>
        /// <param name="baseContext">Pass this.HttpContext</param>
        /// <param name="cookieName">Pass the default cookie name.</param>
        public CookieProcessor(string cookieName) {
            CookieName = cookieName;
        }

        #endregion

        // Cookies add will add duplicate cookies.
        // Cookies set will only add unique cookies.


        #region Save Cookies

        /// <summary>
        /// Add object to cookies whether exist or not.
        /// </summary>
        /// <param name="Object"></param>
        public void Save(object Object) {
            Save(Object, CookieName, false, defaultExpiration);
        }

        /// <summary>
        /// Add object to cookies whether exist or not.
        /// Default expiration is +5 Hours.
        /// </summary>
        /// <param name="Object">Pass the object.</param>
        /// <param name="cookie">Name of the cookie</param>
        public void Save(object Object, string cookie) {
            Save(Object, cookie, false, defaultExpiration);
        }

        /// <summary>
        /// Save a single object as cookie.
        /// Save in Response.
        /// Default expiration is +5 Hours.
        /// </summary>
        /// <param name="Object">Pass the object</param>
        /// <param name="checkBeforeExist">Don't save if already exist.</param>
        public void Save(object Object, bool checkBeforeExist) {
            Save(Object, null, checkBeforeExist, defaultExpiration);
        }

        /// <summary>
        /// Save a single object as cookie.
        /// Save in Response.
        /// </summary>
        /// <param name="Object">Pass the object</param>
        /// <param name="cookieName">Cookie name , pass null if constructor CookieName is valid.</param>
        /// <param name="expiration"></param>
        public void Save(object Object, string cookieName, DateTime expiration) {
            Save(null, cookieName, false, expiration);
        }

        /// <summary>
        /// Save a single object as cookie.
        /// Save in Response.
        /// </summary>
        /// <param name="Object">Pass the object</param>
        /// <param name="cookieName">Cookie name , pass null if constructor CookieName is valid.</param>
        /// <param name="checkBeforeExist">True: Don't save if already exist. </param>
        /// <param name="expiration"></param>
        public void Save(object Object, string cookieName, bool checkBeforeExist, DateTime expiration) {
            if (cookieName == null) {
                cookieName = CookieName;
            }
            var httpCookie = new HttpCookie(cookieName) {
                Expires = expiration
            };
            //createdCookie = true;
            //}
            var isSupport = DataTypeSupport.Support(Object);

            //is not null and don't support = complex
            if (Object != null && !isSupport) {
                //if not a primitive type and thus complex class
                List<ObjectProperty> list = ObjectToArray.ObjectToArrary.Get(Object);
                foreach (var item in list) {
                    if (DataTypeSupport.Support(item.Value) && item.Value != null) {
                        httpCookie[item.Name] = item.Value.ToString();
                    } else {
                        httpCookie[item.Name] = null;
                    }
                }
            } else if (Object != null && isSupport) {
                // object exist but not a complex type of object.
                httpCookie.Value = (string)Object;
            }
            HttpContext.Current.Response.Cookies.Set(httpCookie); //only add unique cookies
        }


        #endregion

        #region Get Cookie -> Same as Reading
        /// <summary>
        /// Get Default cookie string value.
        /// </summary>
        /// <returns>GetDefault cookie string value.</returns>
        public string Get() {
            return ReadString();
        }

        /// <summary>
        /// Get Default cookie string value.
        /// </summary>
        /// <returns>GetDefault cookie string value.</returns>
        public string Get(string cookieName) {
            return ReadString(cookieName);
        }
        #endregion

        #region Set Cookie -> Same as Saving
        /// <summary>
        /// Save cookie. +5 hours expiration.
        /// </summary>
        /// <returns>GetDefault cookie string value.</returns>
        public void Set(string str, string cookieName) {
            Save(str, cookieName);
        }

        /// <summary>
        /// Save cookie. +5 hours expiration.
        /// </summary>
        /// <returns>GetDefault cookie string value.</returns>
        public void Set(string str, string cookieName, DateTime expires) {
            Save(str, cookieName, expires);
        }
        #endregion

        #region Read Cookie

        /// <summary>
        /// Read cookie from request.
        /// </summary>
        /// <returns>Returns string or null.</returns>
        public NameValueCollection Read() {
            var httpCookie = HttpContext.Current.Request.Cookies[CookieName];
            if (httpCookie != null) {
                if (httpCookie.Values.Count > 1) {
                    // complex type not a value.
                    return httpCookie.Values;
                } else {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Read cookie from request.
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns>Return object or null.</returns>
        public NameValueCollection Read(string cookieName) {
            if (cookieName == null) {
                cookieName = CookieName;
            }
            var httpCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (httpCookie != null) {
                if (httpCookie.Values.Count > 1) {
                    // complex type not a value.
                    return httpCookie.Values;
                } else {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Read cookie from request.
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns>Returns string or null.</returns>
        public string ReadString(string cookieName) {
            var httpCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (httpCookie != null) {
                if (httpCookie.Values.Count == 1) {
                    // complex type not a value.
                    return httpCookie.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Read cookie from request.
        /// </summary>
        /// <returns>Returns string or null.</returns>
        public string ReadString() {
            var httpCookie = HttpContext.Current.Request.Cookies[CookieName];
            if (httpCookie != null) {
                if (httpCookie.Values.Count == 1) {
                    // complex type not a value.
                    return httpCookie.Value;
                }
            }
            return null;
        }


        /// <summary>
        /// Read cookie from request.
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns>Returns Boolean.</returns>
        public bool ReadBool(string cookieName) {
            string n = ReadString(cookieName);
            if (!String.IsNullOrWhiteSpace(n)) {
                bool res = false;
                if (bool.TryParse(n, out res)) {
                    return res;
                }
            }
            return false;
        }

        /// <summary>
        /// Read cookie from request.
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns>Returns decimal.</returns>
        public decimal ReadDecimal(string cookieName) {
            string n = ReadString(cookieName);
            decimal res = 0;
            if (!String.IsNullOrWhiteSpace(n)) {
                if (Decimal.TryParse(n, out res)) {
                    return res;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read cookie from request.
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns>Returns long</returns>
        public long ReadLong(string cookieName) {
            string n = ReadString(cookieName);
            long res = 0;
            if (!String.IsNullOrWhiteSpace(n)) {
                if (long.TryParse(n, out res)) {
                    return res;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read cookie from request.
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns>Returns int.</returns>
        public int ReadInt(string cookieName) {
            string n = ReadString(cookieName);
            int res = 0;
            if (!String.IsNullOrWhiteSpace(n)) {
                if (int.TryParse(n, out res)) {
                    return res;
                }
            }
            return 0;
        }

        /// <summary>
        /// Read cookie from request.
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns>Return Date time or null.</returns>
        public DateTime? ReadDateTime(string cookieName) {
            string n = ReadString(cookieName);
            DateTime res;
            if (!String.IsNullOrWhiteSpace(n)) {
                if (DateTime.TryParse(n, out res)) {
                    return res;
                }
            }
            return null;
        }


        #endregion

        #region Remove Cookies
        public void Remove(string name) {
            if (HttpContext.Current.Request.Cookies[name] != null) {
                HttpContext.Current.Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
            }
        }
        #endregion
    }
}