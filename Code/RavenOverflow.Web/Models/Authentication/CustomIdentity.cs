using System.Security.Principal;

namespace RavenOverflow.Web.Models.Authentication
{
    public class CustomIdentity : GenericIdentity, ICustomIdentity
    {
        public CustomIdentity(string userId, string displayName)
            : base(string.IsNullOrEmpty(displayName) ? string.Empty : displayName, "Forms")
        {
            UserId = userId;
        }

        #region Implementation of CustomIdentity

        public string UserId { get; private set; }

        #endregion
    }
}