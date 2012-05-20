using Raven.Client;
using Raven.Client.Document;
using Raven.Client.MvcIntegration;
using RavenOverflow.Web.RavenDb;
using RavenOverflow.Web.RavenDb.Indexes;
using StructureMap.Configuration.DSL;

namespace RavenOverflow.Web.DependencyResolution
{
    public class RavenDbRegistry : Registry
    {
        public RavenDbRegistry(string connectionStringName)
        {
            For<IDocumentStore>()
                .Singleton()
                .Use(x =>
                     {
                         var documentStore = new DocumentStore {ConnectionStringName = connectionStringName};
                         documentStore.InitializeWithDefaults();

                         // Create any Facets.
                         RavenFacetTags.CreateFacets(documentStore);

                         // Wire up the RavenDb profiler.
                         // This is -very- MVC specific, of course. You wouldn't find this in the Tests.
                         RavenProfiler.InitializeFor(documentStore);

                         return documentStore;
                     }
                )
                .Named("RavenDB Document Store.");

            For<IDocumentSession>()
                .HttpContextScoped()
                .Use(x =>
                     {
                         var documentStore = x.GetInstance<IDocumentStore>();
                         return documentStore.OpenSession();
                     })
                .Named("RavenDb Session -> per Http Request.");
        }
    }
}