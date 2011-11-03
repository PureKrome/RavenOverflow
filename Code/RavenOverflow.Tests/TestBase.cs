using System;
using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using RavenOverflow.Core.Entities;
using RavenOverflow.FakeData;
using RavenOverflow.Web.Indexes;

namespace RavenOverflow.Tests
{
    public abstract class TestBase
    {
        private IDocumentStore _documentStore;

        protected IDocumentStore DocumentStore
        {
            get
            {
                // Initialise the Store.
                _documentStore = new EmbeddableDocumentStore {RunInMemory = true};
                _documentStore.Initialize();

                // Index initialisation.
                IndexCreation.CreateIndexes(typeof (RecentPopularTags).Assembly, _documentStore);

                // Create any Facets.
                RavenFacetTags.CreateFacets(_documentStore);

                // Create our Seed Data.
                CreateSeedData(_documentStore);

                return _documentStore;
            }
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
    }
}