using RavenOverflow.Core.Entities;

namespace RavenOverflow.Core.Services
{
    public interface IQuestionService
    {
        Question Store(Question question);
    }
}