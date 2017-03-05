using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NotifManager.Utility
{
    public class AuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        private SessionHelper _session = new SessionHelper(System.Web.HttpContext.Current.Session);

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                // Don't check for authorization as AllowAnonymous filter is applied to the action or controller
                return;
            }

            // Check for authorization
            if (_session.CurrentClient.Id == Guid.Empty)
            {
                //filterContext.Result = filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
            }
        }
    }
}