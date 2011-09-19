using System.Linq;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Core.Services;

namespace RavenOverflow.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IDocumentSession _documentSession;

        public QuestionService(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        #region Implementation of IQuestionService

        public IQueryable<Question> Query()
        {
            return _documentSession.Query<Question>();
        }

        public void Store(Question question)
        {
            Condition.Requires(question).IsNotNull();

            _documentSession.Store(question);
        }

        #endregion
    }
}