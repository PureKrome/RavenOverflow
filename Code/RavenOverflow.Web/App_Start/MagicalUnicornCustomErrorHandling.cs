using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(RavenOverflow.Web.App_Start.MagicalUnicornCustomErrorHandling), "PreStart")]

namespace RavenOverflow.Web.App_Start
{
    public static class MagicalUnicornCustomErrorHandling 
    {
        public static void PreStart() 
        {
            // Lets wire up our two Error Handling routes, really early.

            // This will be pushed down to route #2.
            RouteTable.Routes.Insert(0, new Route("ServerError",
                                                  new RouteValueDictionary(
                                                      new {controller = "Error", action = "ServerError"}),
                                                  new MvcRouteHandler()));
            // And now our first route.
            RouteTable.Routes.Insert(0, new Route("NotFound",
                                                  new RouteValueDictionary(
                                                      new {controller = "Error", action = "NotFound"}),
                                                  new MvcRouteHandler()));
        }
    }
}