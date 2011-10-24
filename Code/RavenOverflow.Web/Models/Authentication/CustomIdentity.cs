using System.Security.Principal;

namespace RavenOverflow.Web.Models.Authentication
{
    public class CustomIdentity : GenericIdentity, ICustomIdentity
    {
        public CustomIdentity(int userId, string displayName)
            : base(string.IsNullOrEmpty(displayName) ? string.Empty : displayName, "Forms")
        {
            UserId = userId;
        }

        #region Implementation of CustomIdentity

        public int UserId { get; private set; }

        #endregion
    }
}