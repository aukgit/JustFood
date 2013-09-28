using System.Web.Mvc;
using JustFood.Modules.Cache;
using JustFood.Modules.StaticContents;

namespace JustFood.Areas.Admin.Controllers {
    public class HomeController : Controller {
        public HomeController() {
            
        }
        public ActionResult Index() {
            //var tx = Statics.Caches.Get(CacheNames.Sale);
            return View();
        }
    }
}