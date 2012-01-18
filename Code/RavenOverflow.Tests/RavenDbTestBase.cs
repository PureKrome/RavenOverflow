using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CuttingEdge.Conditions;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Client.Listeners;
using RavenOverflow.Core.Entities;
using RavenOverflow.FakeData;
using RavenOverflow.Web.Indexes;

namespace RavenOverflow.Tests
{
    public abstract class RavenDbTestBase : IDisposable
    {
        protected RavenDbTestBase()
        {
            InitaliseDocumentStore();
        }

        protected IDocumentStore DocumentStore { get; private set; }

        private void InitaliseDocumentStore()
        {
            // Initialise the Store.
            var documentStore = new EmbeddableDocumentStore
                                {
                                    RunInMemory = true
                                };
            documentStore.Initialize();

            // Force query's to wait for index's to catch up. Unit Testing only :P
            documentStore.RegisterListener(new NoStaleQueriesListener());

            // Index initialisation.
            IndexCreation.CreateIndexes(typeof(RecentPopularTags).Assembly, documentStore);

            // Create any Facets.
            RavenFacetTags.CreateFacets(documentStore);

            // Create our Seed Data.
            CreateSeedData(documentStore);

            DocumentStore = documentStore;
        }

        public void Dispose()
        {
            if (DocumentStore == null)
            {
                return;
            }

            // Check to see if we had any errors, like an index wasn't found or something.
            var statistics = DocumentStore.DatabaseCommands.GetStatistics();
            if (statistics.Errors != null && statistics.Errors.Length > 0)
            {
                // Lets write out each error to the unit test console.
                foreach (var serverError in statistics.Errors)
                {
                    Debug.WriteLine(serverError.Error);
                    throw new InvalidOperationException("There were some errors with the Document Store.");
                }
            }

            DocumentStore.Dispose();
        }

        private static void CreateSeedData(IDocumentStore documentStore)
        {
            Condition.Requires(documentStore).IsNotNull();

            // Users.
            ICollection<User> users = FakeUsers.CreateFakeUsers(50);

            // Questions.
            ICollection<Question> questions = FakeQuestions.CreateFakeQuestions(users.Select(x => x.Id).ToList());

            using (IDocumentSession documentSession = documentStore.OpenSession())
            {
                StoreFakeEntities(users, documentSession);
                StoreFakeEntities(questions, documentSession);

                documentSession.SaveChanges();
            }
        }

        private static void StoreFakeEntities(IEnumerable<RootAggregate> entities, IDocumentSession session)
        {
            // Dont' use Condition.Requires for entities becuase it might enumerate through it.
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            Condition.Requires(session).IsNotNull();

            foreach (RootAggregate entity in entities)
            {
                session.Store(entity);
            }
        }

        #region Nested type: NoStaleQueriesListener

        public class NoStaleQueriesListener : IDocumentQueryListener
        {
            #region Implementation of IDocumentQueryListener

            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
                queryCustomization.WaitForNonStaleResults();
            }

            #endregion
        }

        #endregion
    }
}