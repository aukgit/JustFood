using System.Web;
using JustFood.Modules.Cookie;

namespace JustFood.Modules.Message {
    public static class MessageSetter {
        /// <summary>
        /// Set information type message
        /// </summary>
        /// <param name="msg"></param>
        public static void Set(string msg) {
            SetInfo(msg);
        }

        /// <summary>
        /// Set message to display top of the screen.
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="type">Message type from MessageTypes.Type</param>
        public static void Set(string msg, string type = null) {
            StaticContents.Statics.Cookies.Set(msg,CookiesNames.MessageSetterMsg);   
            StaticContents.Statics.Cookies.Set(type,CookiesNames.MessageSetterType);   
        }
        public static void SetError(string msg) {
            StaticContents.Statics.Cookies.Set(msg, CookiesNames.MessageSetterMsg);
            StaticContents.Statics.Cookies.Set(MessageTypes.Error, CookiesNames.MessageSetterType);
        }
        public static void SetPositive(string msg) {
            StaticContents.Statics.Cookies.Set(msg, CookiesNames.MessageSetterMsg);
            StaticContents.Statics.Cookies.Set(MessageTypes.Positive, CookiesNames.MessageSetterType);
        }

        public static void SetWarning(string msg) {
            StaticContents.Statics.Cookies.Set(msg, CookiesNames.MessageSetterMsg);
            StaticContents.Statics.Cookies.Set(MessageTypes.Warning, CookiesNames.MessageSetterType);
        }


        public static void SetInfo(string msg) {
            StaticContents.Statics.Cookies.Set(msg, CookiesNames.MessageSetterMsg);
            StaticContents.Statics.Cookies.Set(MessageTypes.Information, CookiesNames.MessageSetterType);
        }
    }
}