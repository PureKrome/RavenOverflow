using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using CuttingEdge.Conditions;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Client.Listeners;
using RavenOverflow.Core.Entities;
using RavenOverflow.FakeData;
using RavenOverflow.Web.Indexes;
using RavenOverflow.Web.Models;

namespace RavenOverflow.Tests
{
    public abstract class RavenDbFactBase : IDisposable
    {
        private IDocumentStore _documentStore;
        private readonly bool _isSeedDataInitialized;

        protected IList<Type> IndexesToExecute { get; set; }

        protected RavenDbFactBase()
        {
            _isSeedDataInitialized = true;
        }

        protected IDocumentStore DocumentStore
        {
            get
            {
                if (_documentStore != null)
                {
                    return _documentStore;
                }

                var documentStore = new EmbeddableDocumentStore
                                    {
                                        RunInMemory = true
                                    };
                documentStore.Initialize();

                // Force query's to wait for index's to catch up. Unit Testing only :P
                documentStore.RegisterListener(new NoStaleQueriesListener());

                // Index initialisation.
                if (IndexesToExecute != null)
                {
                    var indexes = (from type in IndexesToExecute
                                   where type.IsSubclassOf(typeof(AbstractIndexCreationTask))
                                   select type).ToArray();

                    IndexCreation.CreateIndexes(new CompositionContainer(new TypeCatalog(indexes)), documentStore);
                }

                // Create any Facets.
                RavenFacetTags.CreateFacets(documentStore);

                // Create our Seed Data.
                if (_isSeedDataInitialized)
                {
                    CreateSeedData(documentStore);
                }

                // Make sure all our indexes are not stale.
                documentStore.WaitForStaleIndexesToComplete();

                // Now lets check to make sure the seeding didn't error.
                documentStore.AssertDocumentStoreErrors();

                _documentStore = documentStore;

                return _documentStore;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (DocumentStore == null || DocumentStore.WasDisposed)
            {
                return;
            }

            // Assert for any errors.
            DocumentStore.AssertDocumentStoreErrors();

            // Clean up.
            DocumentStore.Dispose();
        }

        #endregion

        private static void CreateSeedData(IDocumentStore documentStore)
        {
            Condition.Requires(documentStore).IsNotNull();

            using (IDocumentSession documentSession = documentStore.OpenSession())
            {
                // Users.
                ICollection<User> users = FakeUsers.CreateFakeUsers(50);
                StoreFakeEntities(users, documentSession);

                // Questions.
                ICollection<Question> questions = FakeQuestions.CreateFakeQuestions(users.Select(x => x.Id).ToList());
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
            #region IDocumentQueryListener Members

            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
                queryCustomization.WaitForNonStaleResults();
            }

            #endregion
        }

        #endregion
    }
}