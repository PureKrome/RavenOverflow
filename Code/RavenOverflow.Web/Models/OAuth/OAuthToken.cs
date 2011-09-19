using System.Linq;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace RavenOverflow.Web.Models.OAuth
{
    public class OAuthToken
    {
        private const string IdKey = "OAuthTokens/";
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value.StartsWith(IdKey) ? value : string.Format("{0}{1}", IdKey, value); }
        }
        public string SecretToken { get; set; }
        public TokenType TokenType { get; set; }
        public string Service { get; set; }
        public long UserId { get; set; }
    }

    public static class OAuthTokenFilters
    {
        public static IQueryable<OAuthToken> WithId(this IQueryable<OAuthToken> value, string publicToken)
        {
            return value.Where(x => x.Id == publicToken);
        }
    }
}