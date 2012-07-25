using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RavenOverflow.Web.AutoMapper;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Web
{
    // ReSharper disable InconsistentNaming

    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        private static void RegisterRazorViewEngine()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);

            RegisterRazorViewEngine();

            RegisterRoutes(RouteTable.Routes);

            // Configure AutoMapper mappings.
            AutoMapperBootstrapper.ConfigureMappings();
        }

        protected void Application_AuthenticateRequest()

        {
            CustomFormsAuthentication.AuthenticateRequestDecryptCustomFormsAuthenticationTicket(Context);
        }
    }

    // ReSharper restore InconsistentNaming
}