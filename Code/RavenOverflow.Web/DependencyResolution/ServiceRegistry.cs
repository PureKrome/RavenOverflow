using RavenOverflow.Core.Services;
using RavenOverflow.Services;
using StructureMap.Configuration.DSL;

namespace RavenOverflow.Web.DependencyResolution
{
    public class ServiceRegistry : Registry
    {
        public ServiceRegistry()
        {
            For<IQuestionService>().Use<QuestionService>()
                .Named("Question service.");
        }
    }
}