using RavenOverflow.Web.Models.ViewModels;

namespace RavenOverflow.Web.Models
{
    // ReSharper disable InconsistentNaming

    public class _LayoutViewModel
    {
        public _LayoutViewModel(ClaimsUser claimsUser)
        {
            if (claimsUser != null)
            {
                AuthenticationViewModel = new AuthenticationViewModel(claimsUser);
            }
        }

        public AuthenticationViewModel AuthenticationViewModel { get; private set; }
        public string Header { get; set; }
    }

    // ReSharper restore InconsistentNaming
}