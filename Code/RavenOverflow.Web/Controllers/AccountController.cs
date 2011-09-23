using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CuttingEdge.Conditions;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Facebook;
using Facebook.Web;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Services;
using RavenOverflow.Web.Models;
using RavenOverflow.Web.Models.OAuth;

namespace RavenOverflow.Web.Controllers
{
    public class AccountController : AbstractController
    {
        // https://dev.twitter.com/apps/1241248/show
        // http://blog.bobcravens.com/2010/08/openid-and-oauth-using-dotnetopenauth-in-asp-net-mvc/

        //static private readonly OpenIdRelyingParty Openid = new OpenIdRelyingParty();
        //static private readonly InMemoryTokenManager TokenManager = new InMemoryTokenManager("2zk4PueIkCLKij4MW92Q", "Ret6t7PUZPB7ACa2i35QA3qoYCDoPciqQSdtrUV0VY0");

        private readonly IConsumerTokenManager _consumerTokenManager;

        private const string FacebookAppId = "280239422005855";
        private const string FacebookSecret = "f1925508fc615d387188d3869c8c553c";
        private const string FacebookOAuthUrl = "https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&state={2}";
        private const string FacebookLogoffUrl = "";


        public AccountController(IDocumentStore documentStore, IConsumerTokenManager consumerTokenManager) : base(documentStore)
        {
            _consumerTokenManager = consumerTokenManager;
        }

        #region Authentication

        [ActionName("Authenticate")]
        [HttpGet]
        public ActionResult AuthenticateIndex()
        {
            return View("AuthenticateIndex");
        }

        #region Twitter Authentication

        //[HttpPost]
        public ActionResult AuthenticateWithTwitter()
        {
            if (Request.Url == null)
            {
                return View("AuthenticateIndex");
            }

            var twitter = new WebConsumer(TwitterConsumer.ServiceDescription, _consumerTokenManager);
            var callBackUrl = CreateUri(Request.Url, "Account/TwitterOAuthCallback");
            twitter.Channel.Send(twitter.PrepareRequestUserAuthorization(callBackUrl, null, null));

            return RedirectToAction("Authenticate");
        }

        public ActionResult TwitterOAuthCallback()
        {
            var twitter = new WebConsumer(TwitterConsumer.ServiceDescription, _consumerTokenManager);
            AuthorizedTokenResponse accessTokenResponse = twitter.ProcessUserAuthorization();
            if (accessTokenResponse != null)
            {
                string userName = accessTokenResponse.ExtraData["screen_name"];
                //return CreateUser(userName, null, null, null);
            }
            //_logger.Error("OAuth: No access token response!");
            return RedirectToAction("Authenticate");
        }

        #endregion

        #region Facebook Authentication

        // REFERENCE: https://developers.facebook.com/docs/authentication/
        //            http://facebooksdk.codeplex.com/discussions/244568

        public ActionResult AuthenticateWithFacebook()
        {
            var facebookOAuthClient = FacebookOAuthClient;
            Dictionary<string, object> parameters = new Dictionary<string, object> {{"scope", "email"}};
            var facebookAuthenticationUri = facebookOAuthClient.GetLoginUrl(parameters);
            return Redirect(facebookAuthenticationUri.ToString());
        }

        [RavenActionFilter]
        public ActionResult FacebookAuthenticationCallback()
        {
            FacebookOAuthResult facebookOAuthResult;
            if (FacebookOAuthResult.TryParse(Request.Url, out facebookOAuthResult))
            {
                if (facebookOAuthResult.IsSuccess)
                {
                    return AcceptFacebookOAuthToken(facebookOAuthResult);
                }
            }

            return Content("failed to authenticate.");
        }

        #endregion

        #endregion

        private Uri FacebookAuthorizationCallBackUri
        {
            get { return new Uri(Url.Action("FacebookAuthenticationCallback", null, null, "http")); }
        }

        private FacebookOAuthClient FacebookOAuthClient
        {
            get
            {
                return new FacebookOAuthClient()
                                    {
                                        AppId = FacebookAppId,
                                        AppSecret = FacebookSecret,
                                        RedirectUri = FacebookAuthorizationCallBackUri
                                    };
            }
        }

        private static Uri CreateUri(Uri requestUri, string resource)
        {
            Condition.Requires(requestUri).IsNotNull();
            Condition.Requires(resource).IsNotNull(); // Can be empty, though.

            return new Uri(string.Format("{0}://{1}//{2}", requestUri.Scheme, requestUri.Authority, resource));
        }

        private ActionResult AcceptFacebookOAuthToken(FacebookOAuthResult facebookOAuthResult)
        {
            Condition.Requires(facebookOAuthResult).IsNotNull();

            // Grab the code.
            var code = facebookOAuthResult.Code;

            // Grab the access token.
            var facebookOAuthClient = FacebookOAuthClient;
            dynamic result = facebookOAuthClient.ExchangeCodeForAccessToken(code);

            OAuthData oauthData = new OAuthData
                                         {
                                             OAuthProvider = OAuthProvider.Facebook,
                                             AccessToken = result.access_token,
                                             ExpiresOn = DateTime.UtcNow.AddSeconds(result.expires)
                                         };

            // Now grab their info.
            var facebookWebClient = new FacebookWebClient(oauthData.AccessToken);
            dynamic facebookUser = facebookWebClient.Get("me");
            oauthData.Id = facebookUser.id;

            // Now associate this facebook user to an existing user OR create a new one.
            UserService userService = new UserService(DocumentSession);
            var user = userService.CreateOrUpdate(oauthData, facebookUser.username, facebookUser.name, facebookUser.email);

            return Json(user, JsonRequestBehavior.AllowGet);

        }
    }
}
