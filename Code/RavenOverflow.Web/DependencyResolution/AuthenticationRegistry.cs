using CuttingEdge.Conditions;
using RavenOverflow.Web.Models.Authentication;
using StructureMap.Configuration.DSL;

namespace RavenOverflow.Web.DependencyResolution
{
    public class AuthenticationRegistry : Registry
    {
        public AuthenticationRegistry(string facebookId, string facebookSecret)
        {
            Condition.Requires(facebookId).IsNotNullOrEmpty();
            Condition.Requires(facebookSecret).IsNotNullOrEmpty();

            For<ICustomFormsAuthentication>().Use<CustomFormsAuthentication>()
                .Named("Custom Forms Authentication instance.");
            For<IOAuthAuthentication>().Use<OAuthAuthentication>()
                .Ctor<string>("facebookAppId").Is(facebookId)
                .Ctor<string>("facebookSecret").Is(facebookSecret)
                .Named("OAuth Authentication instance.");
        }
    }
}