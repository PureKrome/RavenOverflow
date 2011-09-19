using RavenOverflow.Core.Services;
using RavenOverflow.Services;
using StructureMap.Configuration.DSL;

namespace RavenOverflow.Web.DependencyResolution
{
    public class ServicesRegistry : Registry
    {
        public ServicesRegistry()
        {
            For<IUserService>().Use<UserService>().Named("User Service.");
            For<IQuestionService>().Use<QuestionService>().Named("Question Service.");
        }
    }
}