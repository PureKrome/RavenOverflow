using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using RavenOverflow.Web.Indexes;
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

                             // Index initialisation.
                             IndexCreation.CreateIndexes(typeof(RecentTags).Assembly, store);

                             return store;
                         }
                )
                .Named("RavenDB Document Store.");
        }
    }
}