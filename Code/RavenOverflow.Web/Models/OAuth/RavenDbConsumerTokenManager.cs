using System.Linq;
using CuttingEdge.Conditions;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Raven.Client;

namespace RavenOverflow.Web.Models.OAuth
{
    public class RavenDbConsumerTokenManager : IConsumerTokenManager
    {
        // Reference: http://msdn.microsoft.com/en-us/library/ff512786.aspx

        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly IDocumentSession _documentSession;

        public RavenDbConsumerTokenManager(IDocumentSession documentSession, string consumerKey, string consumerSecret)
        {
            _documentSession = documentSession;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        #region Implementation of ITokenManager

        public string GetTokenSecret(string token)
        {
            return _documentSession.Query<OAuthToken>()
                .WithId(token)
                .Select(x => x.SecretToken)
                .SingleOrDefault();
        }

        public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
        {
            StoreTokenData(new OAuthToken
            {
                Id = response.Token,
                SecretToken = response.TokenSecret,
                TokenType = TokenType.RequestToken
            });
            _documentSession.SaveChanges();
        }

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken,
                                                             string accessTokenSecret)
        {
            RemoveToken(requestToken);
            StoreTokenData(new OAuthToken
            {
                Id = accessToken,
                SecretToken = accessTokenSecret,
                TokenType = TokenType.AccessToken
            });
            _documentSession.SaveChanges();
        }

        public TokenType GetTokenType(string token)
        {
            return _documentSession.Query<OAuthToken>()
                .WithId(token)
                .Select(x => x.TokenType)
                .SingleOrDefault();
        }

        #endregion

        #region Implementation of IConsumerTokenManager

        public string ConsumerKey
        {
            get { return _consumerKey; }
        }

        public string ConsumerSecret
        {
            get { return _consumerSecret; }
        }

        #endregion

        private void StoreTokenData(OAuthToken oAuthToken)
        {
            Condition.Requires(oAuthToken).IsNotNull();
            Condition.Requires(oAuthToken.Id).IsNotNullOrEmpty();
            Condition.Requires(oAuthToken.SecretToken).IsNotNullOrEmpty();
            Condition.Requires(oAuthToken.TokenType).IsNotEqualTo(TokenType.InvalidToken);

            _documentSession.Store(oAuthToken);
        }

        private void RemoveToken(string publicToken)
        {
            _documentSession.Advanced.DatabaseCommands.Delete(new OAuthToken { Id = publicToken }.Id, null);
        }
    }
}