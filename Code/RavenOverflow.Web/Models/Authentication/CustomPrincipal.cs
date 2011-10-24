using System.Security.Principal;

namespace RavenOverflow.Web.Models.Authentication
{
    public class CustomPrincipal : GenericPrincipal
    {
        public CustomPrincipal(ICustomIdentity identity, string[] roles)
            : base(identity, roles)
        {
            Identity = identity;
        }

        public new ICustomIdentity Identity { get; private set; }
    }
}