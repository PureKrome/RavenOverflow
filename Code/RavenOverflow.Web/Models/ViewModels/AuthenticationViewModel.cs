namespace RavenOverflow.Web.Models.ViewModels
{
    public class AuthenticationViewModel
    {
        public AuthenticationViewModel(ClaimsUser claimsUser)
        {
            if (claimsUser == null)
            {
                IsAuthenticated = false;
                return;
            }

            IsAuthenticated = claimsUser.IsAuthenticated;
            Name = claimsUser.Name;
            PictureUri = claimsUser.PictureUri;
        }

        public bool IsAuthenticated { get; private set; }
        public string Name { get; private set; }
        public string PictureUri { get; private set; }
    }
}