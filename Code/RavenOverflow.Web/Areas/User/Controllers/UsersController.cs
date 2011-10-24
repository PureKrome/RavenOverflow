using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CuttingEdge.Conditions;
using Facebook;
using Facebook.Web;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Core.Services;
using RavenOverflow.Services;
using RavenOverflow.Web.Controllers;
using RavenOverflow.Web.Models;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Web.Areas.User.Controllers
{
    public class UsersController : AbstractController
    {
        private readonly string _facebookAppId;
        private readonly string _facebookSecret;
        private readonly ICustomFormsAuthentication _customFormsAuthentication;
        
        public UsersController(IDocumentSession documentSession,
                               ICustomFormsAuthentication customCustomFormsAuthentication,
                               IOAuthAuthentication oAuthAuthentication)
            : base(documentSession)
        {
            Condition.Requires(customCustomFormsAuthentication).IsNotNull();
            Condition.Requires(oAuthAuthentication).IsNotNull();
            Condition.Requires(oAuthAuthentication.FacebookAppId).IsNotNullOrEmpty();
            Condition.Requires(oAuthAuthentication.FacebookSecret).IsNotNullOrEmpty();

            _customFormsAuthentication = customCustomFormsAuthentication;
            _facebookAppId = oAuthAuthentication.FacebookAppId;
            _facebookSecret = oAuthAuthentication.FacebookSecret;
        }

        #region Authentication

        [HttpGet]
        public ActionResult Authenticate()
        {
            return View("Authenticate", new _LayoutViewModel(User.Identity));
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            _customFormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" }); // If I don't provide the area .. I get boom-ski :~(
        }

        #region Facebook Authentication

        // REFERENCE: https://developers.facebook.com/docs/authentication/
        //            http://facebooksdk.codeplex.com/discussions/244568

        private Uri FacebookAuthorizationCallBackUri
        {
            get { return new Uri(Url.Action("FacebookAuthenticationCallback", null, null, "http")); }
        }

        private FacebookOAuthClient FacebookOAuthClient
        {
            get
            {
                return new FacebookOAuthClient
                {
                    AppId = _facebookAppId,
                    AppSecret = _facebookSecret,
                    RedirectUri = FacebookAuthorizationCallBackUri
                };
            }
        }

        [HttpGet]
        public ActionResult AuthenticateWithFacebook()
        {
            FacebookOAuthClient facebookOAuthClient = FacebookOAuthClient;
            var parameters = new Dictionary<string, object> { { "scope", "email" } };
            Uri facebookAuthenticationUri = facebookOAuthClient.GetLoginUrl(parameters);
            return Redirect(facebookAuthenticationUri.ToString());
        }

        [RavenActionFilter]
        public ActionResult FacebookAuthenticationCallback()
        {
            FacebookOAuthResult facebookOAuthResult;
            if (!FacebookOAuthResult.TryParse(Request.Url, out facebookOAuthResult))
            {
                return Content("failed to retrieve a facebook oauth result.");
            }

            if (!facebookOAuthResult.IsSuccess)
            {
                return Content("failed to authenticate.");
            }

            // Nice! Authenticated with Facebook AND we have the OAuth Token.
            // So lets extract any data and create this user account.
            Core.Entities.User user = AcceptFacebookOAuthToken(facebookOAuthResult);

            // Now FormsAuthenticate this user, if they are active.
            if (user == null || !user.IsActive)
            {
                // Failed to authenticate.
                // ToDo: proper error message and clean UI.
                return Content("failed to authenticate - user is inactive.");
            }

            _customFormsAuthentication.SignIn(user.IdAsANumber, user.DisplayName, Response);

            return RedirectToAction("Index", "Home", new {area=""}); // If I don't provide the area .. I get boom-ski :~(
        }

        private Core.Entities.User AcceptFacebookOAuthToken(FacebookOAuthResult facebookOAuthResult)
        {
            Condition.Requires(facebookOAuthResult).IsNotNull();

            // Grab the code.
            string code = facebookOAuthResult.Code;

            // Grab the access token.
            FacebookOAuthClient facebookOAuthClient = FacebookOAuthClient;
            dynamic result = facebookOAuthClient.ExchangeCodeForAccessToken(code);

            var oauthData = new OAuthData
            {
                OAuthProvider = OAuthProvider.Facebook,
                AccessToken = result.access_token,
                ExpiresOn = DateTime.UtcNow.AddSeconds(result.expires)
            };

            // Now grab their info.
            var facebookWebClient = new FacebookWebClient(oauthData.AccessToken);
            dynamic facebookUser = facebookWebClient.Get("me");
            oauthData.Id = facebookUser.id;

            
            // Not sure how to Inject an IUserService because it requires a Session .. which I don't have.
            var userService = new UserService(DocumentSession);
            
            // Now associate this facebook user to an existing user OR create a new one.
            return userService.CreateOrUpdate(oauthData, facebookUser.username, facebookUser.name, facebookUser.email);
        }

        #endregion

        #endregion
    }
}