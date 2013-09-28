using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using JustFood.Modules.Session;

namespace JustFood.Modules.Filters {
    public class AreaAuthorizeAttribute : AuthorizeAttribute {
        private readonly string[] _RestrictedAreas = {"Admin", "Sale"};

        public override void OnAuthorization(AuthorizationContext filterContext) {
            RouteData routeData = filterContext.RouteData;
            // check if user is allowed on this page
            var currentArea = (string) routeData.DataTokens["area"];
            
      
            filterContext.HttpContext.Session[SessionNames.AuthError] = null;
            if (string.IsNullOrEmpty(currentArea)) {
                return;
            }

            if (_RestrictedAreas.All(m => m != currentArea)) {
                return;
            }


            if (filterContext.HttpContext.User.Identity.IsAuthenticated) {
                // if the user doesn't have access to this area

                if (!filterContext.HttpContext.User.IsInRole(currentArea)) {
                    //no access to the area... then add a error msg.
                    filterContext.HttpContext.Session[SessionNames.AuthError] = "You have no right to access " + currentArea + " . Sorry for inconvenience.";
                    HandleUnauthorizedRequest(filterContext);
                }
            }

            // do normal OnAuthorization checks too
            base.OnAuthorization(filterContext);
        }
    }
}