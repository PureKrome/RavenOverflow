using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Services.Interfaces;
using RavenOverflow.Services.Models;

namespace RavenOverflow.Services
{
    public class QuestionService : IQuestionService
    {
        #region IQuestionService Members

        public Question Store(QuestionInputModel questionInputModel, IDocumentSession documentSession)
        {
            Condition.Requires(questionInputModel).IsNotNull();
            Condition.Requires(documentSession).IsNotNull();

            // First, validate the question.
            Validator.ValidateObject(questionInputModel, new ValidationContext(questionInputModel, null, null), true);
            Condition.Requires(questionInputModel.Tags).IsNotNull().IsLongerThan(0);

            Question question = null;
            if (questionInputModel.QuestionId > 0)
            {
                question = documentSession.Load<Question>(questionInputModel.QuestionId);
            }

            if (question == null)
            {
                question = new Question();
            }

            Mapper.Map(questionInputModel, question);

            // Save.
            documentSession.Store(question);

            return question;
        }

        #endregion
    }
}