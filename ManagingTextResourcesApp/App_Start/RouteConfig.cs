using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManagingTextResourcesApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/",
                defaults: new { controller = "Home", action = "Login" }
            );

            routes.MapRoute(
                name: "Editable",
                url: "{controller}/{action}/{hash}",
                defaults: new { controller = "TextResources", action = "Details_EditableLink", hash = UrlParameter.Optional }
            );
        }
    }
}
