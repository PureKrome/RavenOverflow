namespace RavenOverflow.Web.Models.Authentication
{
    public class OAuthAuthentication : IOAuthAuthentication
    {
        public OAuthAuthentication(string facebookAppId, string facebookSecret)
        {
            FacebookAppId = facebookAppId;
            FacebookSecret = facebookSecret;
        }

        #region Implementation of IOAuthAuthentication

        public string FacebookAppId { get; private set; }

        public string FacebookSecret { get; private set; }

        #endregion
    }
}