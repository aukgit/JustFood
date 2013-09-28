using JustFood.Modules.Cache;
using JustFood.Modules.Cookie;
using JustFood.Modules.Query;
using JustFood.Modules.TimeZone;
using JustFood.Modules.UserError;

namespace JustFood.Modules.StaticContents {
    public static class Statics {
        public static CookieProcessor Cookies = new CookieProcessor();
        public static CacheProcessor Caches = new CacheProcessor();
        public static UserInfo UserInfos = new UserInfo();
        public static UserError.ErrorCollector ErrorCollection = new ErrorCollector();

       
    }
}