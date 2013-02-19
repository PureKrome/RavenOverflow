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

            if (!string.IsNullOrEmpty(question.Id))
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
                .Evaluate(x => x.Length >= 5, "A subject is required and needs to be at least 5 characters.");

            Condition.Requires(question.Content)
                .IsNotNullOrEmpty()
                .Evaluate(x => x.Length >= 5, "Some content is required and needs to be at least 5 characters.");

            Condition.Requires(question.Tags)
                .IsNotNull()
                .IsNotEmpty("At least one valid tag is required.");
        }
    }
}