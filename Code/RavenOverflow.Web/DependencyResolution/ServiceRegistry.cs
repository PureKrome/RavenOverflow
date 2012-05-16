using RavenOverflow.Services;
using RavenOverflow.Services.Interfaces;
using StructureMap.Configuration.DSL;

namespace RavenOverflow.Web.DependencyResolution
{
    public class ServiceRegistry : Registry
    {
        public ServiceRegistry()
        {
            For<IQuestionService>().Use<QuestionService>()
                .Named("Custom Forms Authentication instance.");
        }
    }
}