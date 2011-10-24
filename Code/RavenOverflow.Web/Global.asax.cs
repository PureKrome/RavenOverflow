using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.MvcIntegration;
using RavenOverflow.Core.Entities;
using RavenOverflow.FakeData;
using System.Collections.Generic;
using RavenOverflow.Web.Indexes;
using RavenOverflow.Web.Models.Authentication;
using StructureMap;

namespace RavenOverflow.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );
        }

        private static void RegisterRazorViewEngine()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        // ReSharper disable InconsistentNaming
        protected void Application_Start()
        // ReSharper restore InconsistentNaming
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);

            ViewEngines.Engines.Clear();
            RegisterRazorViewEngine();

            RegisterRoutes(RouteTable.Routes);

            // Seed an demo data.
            SeedDocumentStore(ObjectFactory.GetInstance<IDocumentStore>());

            // Create any Facets.
            RavenFacetTags.CreateFacets(ObjectFactory.GetInstance<IDocumentStore>());

            // Wire up the RavenDb profiler.
            RavenProfiler.InitializeFor(ObjectFactory.GetInstance<IDocumentStore>());
        }

        // ReSharper disable InconsistentNaming
        protected void Application_AuthenticateRequest()
        // ReSharper restore InconsistentNaming
        {
            CustomFormsAuthentication.AuthenticateRequestDecryptCustomFormsAuthenticationTicket(Context);
        }

        private static void SeedDocumentStore(IDocumentStore documentStore)
        {
            using (var session = documentStore.OpenSession())
            {
                // Don't add any seed data, if we already have some data in the system.
                var user = session.Query<User>().Take(1).ToList();
                if (user.Any())
                {
                    return;
                }

                var users = FakeUsers.CreateFakeUsers();

                StoreEntites(session, users);

                StoreEntites(session, FakeQuestions.CreateFakeQuestions(users.Select(x => x.Id).ToList()));

                session.SaveChanges();
            }
        }

        private static void StoreEntites(IDocumentSession session, IEnumerable<RootAggregate> entities)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            foreach (var entity in entities)
            {
                session.Store(entity);
            }
        }

        
    }
}