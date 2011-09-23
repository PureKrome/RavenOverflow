using System;

namespace RavenOverflow.Core.Entities
{
    public class OAuthData
    {
        public string Id { get; set; }
        public OAuthProvider OAuthProvider { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}