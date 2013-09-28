using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using JustFood.Modules.Extensions;
using JustFood.Modules.TimeZone;

namespace JustFood.Modules.Helpers {

    public static class HtmlHelpers {
        public static int TruncateLength = AppConfig.TruncateLength;

        public enum DateTimeFormatType {
            Date,
            Time,
            DateTimeSimple,
            DateTimeFull,
            DateTimeShort,
            DateTimeCustom
        }

        public static string Truncate(this HtmlHelper helper, string input, int ? length) {
            if (string.IsNullOrEmpty(input))
                return "";
            if (length == null) {
                length = TruncateLength;
            }
            if (input.Length <= length) {
                return input;
            } else {
                return input.Substring(0, (int)length) + "...";
            }
        }
    
        public static HtmlString ContactFormActionLink(this HtmlHelper helper, string linkName, string title, string AddClass = "") {
            string markup = string.Format("<a id='ContactFormLink' href='/ContactUs' class='{2}' title='{0}'>{1}</a>", title, linkName, AddClass);
            return new HtmlString(markup);
        }


        public static HtmlString Image(this HtmlHelper helper, string img, string alt) {
            string markup = string.Format("<img src='{0}' alt='{1}'/>", VirtualPathUtility.ToAbsolute(img), alt);
            return new HtmlString(markup);
            //return (markup);
        }

        public static HtmlString Image(this HtmlHelper helper, string folder, string img, string ext, string alt) {
            string markup = string.Format("<img src='{0}{1}.{2}' alt='{3}'/>", VirtualPathUtility.ToAbsolute(folder), img, ext, alt);
            //return  new HtmlString(markup);
            return new HtmlString(markup);
        }

        public static HtmlString RouteListItemGenerate(this HtmlHelper helper, string area, string display, string controller, string currentController) {
            string addClass = " class='active' ";
            if (controller != currentController)
                addClass = "";
            string markup = string.Format("<li{0}><a href='{1}'>{2}</a></li>", addClass, "/" + area + "/" + controller, display);
            //return  new HtmlString(markup);
            return new HtmlString(markup);
        }


        public static HtmlString DisplayTimeFormat(this HtmlHelper helper, TimeZoneInfo timeZone, DateTime? dt = null, DateTimeFormatType type = DateTimeFormatType.Date, string format = "") {
            //dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            if (dt == null || timeZone == null) {
                return new HtmlString("");
            }
            if (format == "") {
                switch (type) {
                    case DateTimeFormatType.Date:
                        format = "dd-MMM-yyyy";
                        break;
                    case DateTimeFormatType.DateTimeSimple:
                        format = "dd-MMM-yyyy hh:mm:ss tt";
                        break;
                    case DateTimeFormatType.DateTimeFull:
                        format = "MMMM dd, yyyy hh:mm:ss tt";
                        break;
                    case DateTimeFormatType.DateTimeShort:
                        format = "d-MMM-yy hh:mm:ss tt";
                        break;
                    case DateTimeFormatType.Time:
                        format = "hh:mm:ss tt";
                        break;
                    default:
                        break;
                }
            }
            return new HtmlString(Zone.GetTime(timeZone, dt, format));
        }

        public static HtmlString DisplayDate(this HtmlHelper helper, TimeZoneInfo timeZone, DateTime? dt = null) {
            if (dt == null || timeZone == null) {
                return new HtmlString("");
            }
            return new HtmlString(Zone.GetDate(timeZone, dt));
        }

        public static HtmlString DisplayTime(this HtmlHelper helper, TimeZoneInfo timeZone, DateTime? dt = null) {
            if (dt == null || timeZone == null) {
                return new HtmlString("");
            }
            return new HtmlString(Zone.GetTime(timeZone, dt));
        }

        public static HtmlString DisplayDateTime(this HtmlHelper helper, TimeZoneInfo timeZone, DateTime? dt = null) {
            if (dt == null || timeZone == null) {
                return new HtmlString("");
            }
            return new HtmlString(Zone.GetTime(timeZone, dt));
        }

        public static HtmlString SubmitButton(this HtmlHelper helper, string buttonName ="Save" ,string alertMessage = "Are you sure about this action?"){
            string sendbtn =  String.Format(
             "<input type=\"submit\" value=\"{0}\" onClick=\"return confirm('{1}');\" />",
             buttonName, alertMessage);
            return new HtmlString(sendbtn);
        }

        /// <summary>
        /// JqueryMobile BackButton
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="buttonName"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static HtmlString BackButton(this HtmlHelper helper, string buttonName = "Back", bool isMini = false , string icon = "arrow-l") {
            string mini = (isMini)
                              ? "data-mini='true'"
                              : "";
            string backbtn = "<a href='#' data-role='button' class = 'back-button' data-rel='back' data_icon='" + icon + "' " + mini + " >" + buttonName + "</a>";
            return new HtmlString(backbtn);
        }

        //public class DropDownSelect {
        //    public string Text { get; set; }
        //    public string Value { get; set; }
        //}

        //public static HtmlString Dropdown<DropDownSelect>(this HtmlHelper helper, IEnumerable<DropDownSelect> list, object activeValue, string cssClass, string htmlAttr) {
        //    string stringFinal = "";
        //    string rows = "";
        //    foreach (var item in list) {
                
        //    }
        //}
        
    }
}