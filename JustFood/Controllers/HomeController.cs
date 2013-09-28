using System.Web.Mvc;
using JustFood.Modules.Mail;
using JustFood.Modules.StaticContents;

namespace JustFood.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            ViewBag.Message = "Management";
            return View();
        }
    }
}