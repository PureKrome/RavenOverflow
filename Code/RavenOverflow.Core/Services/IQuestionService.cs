using System.Linq;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Core.Services
{
    public interface IQuestionService
    {
        IQueryable<Question> Query();
        void Store(Question question);
    }
}