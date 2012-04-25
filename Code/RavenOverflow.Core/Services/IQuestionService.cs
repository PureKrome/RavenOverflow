using Raven.Client;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Core.Services
{
    public interface IQuestionService
    {
        Question Create(Question question, IDocumentSession documentSession);
    }
}