using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.MvcIntegration;
using RavenOverflow.Core.Entities;
using RavenOverflow.FakeData;
using RavenOverflow.Web.Indexes;
using RavenOverflow.Web.Models;
using RavenOverflow.Web.Models.Authentication;
using RavenOverflow.Web.Models.AutoMapping;
using StructureMap;

namespace RavenOverflow.Web
{
    // ReSharper disable InconsistentNaming

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

            ViewEngines.Engines.Clear();
            RegisterRazorViewEngine();

            RegisterRoutes(RouteTable.Routes);

            // Seed an demo data.
            SeedDocumentStore(ObjectFactory.GetInstance<IDocumentStore>());

            // Create any Facets.
            RavenFacetTags.CreateFacets(ObjectFactory.GetInstance<IDocumentStore>());

            // Wire up the RavenDb profiler.
            RavenProfiler.InitializeFor(ObjectFactory.GetInstance<IDocumentStore>());

            // Configure AutoMapper mappings.
            AutoMapperBootstrapper.ConfigureMappings();
        }

        protected void Application_AuthenticateRequest()

        {
            CustomFormsAuthentication.AuthenticateRequestDecryptCustomFormsAuthenticationTicket(Context);
        }

        private static void SeedDocumentStore(IDocumentStore documentStore)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                // Don't add any seed data, if we already have some data in the system.
                var user = session.Load<User>("users/1");
                if (user != null)
                {
                    return;
                }

                ICollection<User> users = FakeUsers.CreateFakeUsers();

                StoreEntites(session, users);

                StoreEntites(session, FakeQuestions.CreateFakeQuestions(users.Select(x => x.Id).ToList()));

                session.SaveChanges();

                // Wait for all stale indexes to complete.
                while (documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Any())
                {
                    Thread.Sleep(200);
                }
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

            foreach (RootAggregate entity in entities)
            {
                session.Store(entity);
            }
        }
    }

    // ReSharper restore InconsistentNaming
}


