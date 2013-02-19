using StructureMap.Configuration.DSL;
using WorldDomination.Security;

namespace RavenOverflow.Web.DependencyResolution
{
    public class AuthenticationRegistry : Registry
    {
        public AuthenticationRegistry()
        {
            For<ICustomFormsAuthentication>().Use<CustomFormsAuthentication>()
                .Named("Custom Forms Authentication instance.");
        }
    }
}