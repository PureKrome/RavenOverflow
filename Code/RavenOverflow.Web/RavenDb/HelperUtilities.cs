using System;
using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.FakeData;

namespace RavenOverflow.Web.RavenDb
{
    public static class HelperUtilities
    {
        public static void CreateSeedData(IDocumentStore documentStore)
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

                // Make sure all our indexes are not stale.
                documentStore.WaitForStaleIndexesToComplete();
            }
        }

        public static void StoreFakeEntities(IEnumerable<RootAggregate> entities, IDocumentSession session)
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