using System.Web.Mvc;

namespace JustFood.Areas.Sale {
    public class SaleAreaRegistration : AreaRegistration {
        public override string AreaName { get { return "Sale"; } }

        public override void RegisterArea(AreaRegistrationContext context) { context.MapRoute("Sale_default", "Sale/{controller}/{action}/{id}", new {action = "Index", Controller = "Home", id = UrlParameter.Optional}); }
    }
}