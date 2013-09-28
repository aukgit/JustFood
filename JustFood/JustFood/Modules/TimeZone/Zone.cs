using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using JustFood.Modules.Cookie;
using JustFood.Modules.StaticContents;
using DB = JustFood.Models;
using System.Linq;

namespace JustFood.Modules.TimeZone {

    public class Zone {

        #region Fields
        static string defaultTimeFormat = "hh:mm:ss tt";
        static string defaultDateFormat = "dd-MMM-yy";
        static string defaultDateTimeFormat = "dd-MMM-yy hh:mm:ss tt";
        #endregion

        #region Propertise
        /// <summary>
        /// hh:mm:ss tt
        /// </summary>
        public static string TimeFormat { get { return defaultTimeFormat; } set { defaultTimeFormat = value; } }

        /// <summary>
        /// dd-MMM-yy
        /// </summary>
        public static string DateFormat { get { return defaultDateFormat; } set { defaultDateFormat = value; } }

        /// <summary>
        /// dd-MMM-yy
        /// </summary>
        public static string DateTimeFormat { get { return defaultDateTimeFormat; } set { defaultDateTimeFormat = value; } }


        static readonly ReadOnlyCollection<TimeZoneInfo> SystemTimeZones = TimeZoneInfo.GetSystemTimeZones();
        static List<DB.TimeZone> dbTimeZones;
        #endregion

        #region Constructor

        public Zone() {
        }

        public Zone(string timeFormat, string dateFormat = null, string dateTimeFormat = null) {
            defaultTimeFormat = timeFormat;
            if (dateFormat != null) {
                defaultDateFormat = dateFormat;
            }
            if (dateTimeFormat != null) {
                defaultDateTimeFormat = dateTimeFormat;
            }
        }

        #endregion

        #region Application Startup function for database

        public static void SyncZoneInDatabase() {
            using (var db = new DB.JustFoodDBEntities()) {
                var zones = db.TimeZones.ToList();
                var change = false;
                foreach (var timezone in SystemTimeZones) {
                    if (!zones.Any(n => n.TimeZoneInfoId == timezone.Id)) {
                        //not in the database.
                        var timeZoneDb = new DB.TimeZone() {
                            TimeZoneInfoId = timezone.Id,
                            TimeZoneDisplay = timezone.DisplayName
                        };
                        change = true;
                        db.TimeZones.Add(timeZoneDb);
                    }
                }
                if (change)
                    db.SaveChanges();
                dbTimeZones = db.TimeZones.ToList();
            }

        }

        #endregion

        #region Get Zone from Cache

        /// <summary>
        /// Optimized fist check on cache then database.
        /// Get current logged time zone from database or from cache.
        /// </summary>
        /// <returns>Returns time zone of the user.</returns>
        public static TimeZoneInfo Get() {
            if (!HttpContext.Current.User.Identity.IsAuthenticated) {
                return null;
            }
            var log = HttpContext.Current.User.Identity.Name;
            return Get(log);
        }
        /// <summary>
        /// Optimized fist check on cache then database.
        /// Get time zone from database base on username.
        /// </summary>
        /// <param name="log"></param>
        /// <returns>Returns time zone of the user.</returns>
        public static TimeZoneInfo Get(string log) {
            var user = Statics.UserInfos.GetUser(log);
            TimeZoneInfo timeZoneInfo = null;
            timeZoneInfo = GetSavedTimeZone(log);
            if (timeZoneInfo != null) {
                //got time zone from cache.
                return timeZoneInfo;
            }
            if (user != null) {
                var timezoneDb = dbTimeZones.FirstOrDefault(n => n.TimeZoneID == user.TimeZoneID);
                if (timezoneDb != null) {
                    timeZoneInfo = SystemTimeZones.FirstOrDefault(n => n.Id == timezoneDb.TimeZoneInfoId);
                }
                if (timeZoneInfo != null) {
                    // Save the time zone to the cache.
                    SaveTimeZone(timeZoneInfo, log);
                    return timeZoneInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// Get time zone from save cache or cookie of Current user.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        static TimeZoneInfo GetSavedTimeZone() {
            if (!HttpContext.Current.User.Identity.IsAuthenticated) {
                return null;
            }
            var log = HttpContext.Current.User.Identity.Name;
            return GetSavedTimeZone(log);
        }

        /// <summary>
        /// Get time zone from save cache or cookie.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        static TimeZoneInfo GetSavedTimeZone(string log) {
            //save to cookie 
            if (!String.IsNullOrWhiteSpace(log)) {
                var cZone = (TimeZoneInfo)Statics.Caches.Get(CookiesNames.ZoneInfo + log);
                if (cZone == null) {
                    // try cookie.
                    string id = Statics.Cookies.Get(CookiesNames.ZoneInfo);
                    if (id != null) {
                        cZone = SystemTimeZones.FirstOrDefault(n => n.Id == id);
                        return cZone;
                    } else {
                        return null;
                    }
                } else {
                    return cZone; //fast
                }
            }
            return null;
        }
        #endregion

        #region Save Zone in Cache

        /// <summary>
        /// Saved for current logged user.
        /// </summary>
        /// <param name="timeZoneInfo"></param>
        static void SaveTimeZone(TimeZoneInfo timeZoneInfo) {
            if (!HttpContext.Current.User.Identity.IsAuthenticated) {
                return;
            }
            var log = HttpContext.Current.User.Identity.Name;
            SaveTimeZone(timeZoneInfo, log);
        }

        static void SaveTimeZone(TimeZoneInfo timeZoneInfo, string log) {
            if (log == null || timeZoneInfo == null) {
                return;
            }
            //save to cookie 
            Statics.Cookies.Set(timeZoneInfo.Id, CookiesNames.ZoneInfo);
            Statics.Caches.Set(CookiesNames.ZoneInfo + log, timeZoneInfo);
        }

        #endregion

        /// <summary>
        /// Flush cache information about user time-zone.
        /// </summary>
        /// <param name="log"></param>
        public static void RemoveTimeZoneCache(string log) {
            if (log == null) {
                return;
            }
            Statics.Cookies.Remove(CookiesNames.ZoneInfo);
            Statics.Caches.Remove(CookiesNames.ZoneInfo + log);
        }


        #region Dynamic Timing

        public static string GetTimeDynamic() {
            var dynamic = DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute + DateTime.Now.Millisecond;

            return DateTime.Now.ToShortTimeString() + dynamic.ToString() + (dynamic ^ dynamic).ToString();
        }
        #endregion

        #region Get times format based on zone

        /// <summary>
        /// Get date to print as string.
        /// Time zone by user logged in.
        /// It will get the logged user and then get the time-zone and then print.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format">if format null then default format.</param>
        /// <returns>Returns nice string format based on logged user's selected time zone.</returns>
        public static string GetTime(DateTime? dt, string format = null) {
            if (dt == null) {
                return "";
            }
            var dt2 = (DateTime)dt;
            TimeZoneInfo timeZone = Get();
            if (timeZone == null) {
                return "";
            } else {
                //time zone found.
                var newDate = TimeZoneInfo.ConvertTime(dt2, timeZone);
                if (format == null) {
                    format = TimeFormat;
                }
                return newDate.ToString(format);
            }
            return "";
        }

        /// <summary>
        /// Get date to print as string.
        /// Time zone by user logged in.
        /// It will get the logged user and then get the time-zone and then print.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format">if format null then default format.</param>
        /// <returns>Returns nice string format based on logged user's selected time zone.</returns>
        public static string GetDate(DateTime? dt, string format = null) {
            if (dt == null) {
                return "";
            }
            var dt2 = (DateTime)dt;
            TimeZoneInfo timeZone = Get();
            if (timeZone == null) {
                return "";
            } else {
                //time zone found.
                var newDate = TimeZoneInfo.ConvertTime(dt2, timeZone);
                if (format == null) {
                    format = DateFormat;
                }
                return newDate.ToString(format);
            }
            return "";
        }

        /// <summary>
        /// Get date to print as string.
        /// Time zone by user logged in.
        /// It will get the logged user and then get the time-zone and then print.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format">if format null then default format.</param>
        /// <returns>Returns nice string format based on logged user's selected time zone.</returns>
        public static string GetDateTime(DateTime? dt, string format = null) {
            if (dt == null) {
                return "";
            }
            var dt2 = (DateTime)dt;
            TimeZoneInfo timeZone = Get();
            if (timeZone == null) {
                return "";
            } else {
                //time zone found.
                var newDate = TimeZoneInfo.ConvertTime(dt2, timeZone);
                if (format == null) {
                    format = DateTimeFormat;
                }
                return newDate.ToString(format);
            }
            return "";
        }

        #endregion

        #region Based on timezone

        /// <summary>
        /// Get date to print as string.
        /// Time zone by user logged in.
        /// It will get the logged user and then get the time-zone and then print.
        /// </summary>
        /// <param name="timeZone"></param>
        /// <param name="dt"></param>
        /// <param name="format">if format null then default format.</param>
        /// <returns>Returns nice string format based on logged user's selected time zone.</returns>
        public static string GetTime(TimeZoneInfo timeZone, DateTime? dt, string format = null) {
            if (dt == null) {
                return "";
            }
            var dt2 = (DateTime)dt;

            //time zone found.
            var newDate = TimeZoneInfo.ConvertTime(dt2, timeZone);
            if (format == null) {
                format = TimeFormat;
            }
            return newDate.ToString(format);

        }

        /// <summary>
        /// Get date to print as string.
        /// Time zone by user logged in.
        /// It will get the logged user and then get the time-zone and then print.
        /// </summary>
        /// <param name="timeZone"></param>
        /// <param name="dt"></param>
        /// <param name="format">if format null then default format.</param>
        /// <returns>Returns nice string format based on logged user's selected time zone.</returns>
        public static string GetDate(TimeZoneInfo timeZone, DateTime? dt, string format = null) {
            if (dt == null) {
                return "";
            }
            var dt2 = (DateTime)dt;
            //time zone found.
            var newDate = TimeZoneInfo.ConvertTime(dt2, timeZone);
            if (format == null) {
                format = DateFormat;
            }
            return newDate.ToString(format);
        }


        /// <summary>
        /// Get date time to print as string.
        /// Time zone by user logged in.
        /// It will get the logged user and then get the time-zone and then print.
        /// </summary>
        /// <param name="format">if format null then default format.</param>
        /// <returns>Returns nice string format based on logged user's selected time zone.</returns>
        public static string GetCurrentDateTime(string format = null) {
            return GetDateTime(DateTime.Now, format);
        }

        /// <summary>
        /// Get date time to print as string.
        /// Time zone by user logged in.
        /// It will get the logged user and then get the time-zone and then print.
        /// </summary>
        /// <param name="format">if format null then default format.</param>
        /// <returns>Returns nice string format based on logged user's selected time zone.</returns>
        public static string GetCurrentDate(string format = null) {
            return GetDate(DateTime.Now, format);
        }

        /// <summary>
        /// Get date to print as string.
        /// Time zone by user logged in.
        /// It will get the logged user and then get the time-zone and then print.
        /// </summary>
        /// <param name="timeZone"></param>
        /// <param name="dt"></param>
        /// <param name="format">if format null then default format.</param>
        /// <returns>Returns nice string format based on logged user's selected time zone.</returns>
        public static string GetDateTime(TimeZoneInfo timeZone, DateTime? dt, string format = null) {
            if (dt == null) {
                return "";
            }
            var dt2 = (DateTime)dt;

            //time zone found.
            var newDate = TimeZoneInfo.ConvertTime(dt2, timeZone);
            if (format == null) {
                format = DateTimeFormat;
            }
            return newDate.ToString(format);

        }

        #endregion

    }
}