using Raven.Client;
using Raven.Client.Document;
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
                             var store = new DocumentStore
                                             {
                                                 ConnectionStringName = connectionStringName,
                                             };
                             store.Initialize();

                             // TODO: Any Indexes go here.

                             return store;
                         }
                )
                .Named("RavenDB Document Store.");

            For<IDocumentSession>()
                .AlwaysUnique()
                .Use(x =>
                         {
                             var store = x.GetInstance<IDocumentStore>();
                             return store.OpenSession();
                         })
                .Named("RavenDB Session (aka. Unit of Work).");
        }
    }
}