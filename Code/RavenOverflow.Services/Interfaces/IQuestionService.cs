using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Services.Models;

namespace RavenOverflow.Services.Interfaces
{
    public interface IQuestionService
    {
        Question Store(QuestionInputModel questionInputModel, IDocumentSession documentSession);
    }
}