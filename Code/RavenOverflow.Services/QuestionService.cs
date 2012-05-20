using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Core.Services;

namespace RavenOverflow.Services
{
    public class QuestionService : RavenDbBaseService, IQuestionService
    {
        public QuestionService(IDocumentSession documentSession) : base(documentSession)
        {
        }

        #region IQuestionService Members

        public Question Store(Question question)
        {
            Condition.Requires(question).IsNotNull();

            // First, validate the question.
            ValidateQuestion(question);

            if (question.IdAsANumber > 0)
            {
                var existingQuestion = DocumentSession.Load<Question>(question.Id);
                if (existingQuestion != null)
                {
                    existingQuestion.Subject = question.Subject;
                    existingQuestion.Content = question.Content;
                    existingQuestion.Tags = question.Tags;
                    existingQuestion.CreatedByUserId = question.CreatedByUserId;

                    question = existingQuestion;
                }
            }

            // Save.
            DocumentSession.Store(question);

            return question;
        }

        #endregion

        private static void ValidateQuestion(Question question)
        {
            Condition.Requires(question).IsNotNull();

            Condition.Requires(question.Subject)
                .IsNotNullOrEmpty()
                .IsGreaterOrEqual("5", "A subject is missing.");

            Condition.Requires(question.Content)
                .IsNotNullOrEmpty()
                .IsGreaterThan("5", "A question is missing some content.");

            Condition.Requires(question.Tags)
                .IsNotNull()
                .IsNotEmpty("At least one valid tag is required.");
        }
    }
}