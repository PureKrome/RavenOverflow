using System.ComponentModel.DataAnnotations;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Core.Services;

namespace RavenOverflow.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IDocumentStore _documentStore;

        public QuestionService(IDocumentStore documentStore)
        {
            Condition.Requires(documentStore).IsNotNull();

            _documentStore = documentStore;
        }

        #region IQuestionService Members

        public Question Create(Question question)
        {
            Condition.Requires(question).IsNotNull();

            // First, validate the question.
            Validator.ValidateObject(question, new ValidationContext(question, null, null), true);
            Condition.Requires(question.Tags).IsNotNull().IsLongerThan(0);

            // Save.
            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Store(question);
                documentSession.SaveChanges();
            }

            return question;
        }

        #endregion
    }
}