using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuttingEdge.Conditions;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Raven.Client;
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

        private const string FacebookRedirectUrl = "Account/FacebookOAuthCallback";
        private const string FacebookLogoffUrl = "";


        public AccountController(IDocumentStore documentStore, IConsumerTokenManager consumerTokenManager)
            : base(documentStore)
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


        private static Uri CreateUri(Uri requestUri, string resource)
        {
            Condition.Requires(requestUri).IsNotNull();
            Condition.Requires(resource).IsNotNull(); // Can be empty, though.

            return new Uri(string.Format("{0}://{1}//{2}", requestUri.Scheme, requestUri.Authority, resource));
        }
    }
}
