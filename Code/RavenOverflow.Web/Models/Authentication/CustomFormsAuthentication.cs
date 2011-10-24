using System;
using System.Threading;
using System.Web;
using System.Web.Security;

namespace RavenOverflow.Web.Models.Authentication
{
    public class CustomFormsAuthentication : ICustomFormsAuthentication
    {
        #region IFormsAuthenticationService Members

        public void SignIn(int id, string displayName, HttpResponseBase httpResponseBase)
        {
            var userData = new UserData
            {
                Id = id,
                DisplayName = displayName
            };

            string encodedTicket =
                FormsAuthentication.Encrypt(new FormsAuthenticationTicket(1, displayName, DateTime.UtcNow,
                                                                          DateTime.UtcNow.Add(
                                                                              FormsAuthentication.Timeout),
                                                                          true,
                                                                          userData.ToString()));
            var httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encodedTicket);
            httpResponseBase.Cookies.Add(httpCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        #endregion

        public static void AuthenticateRequestDecryptCustomFormsAuthenticationTicket(HttpContext httpContext)
        {
            UserData userData;

            string formsCookieName = FormsAuthentication.FormsCookieName;
            HttpCookie httpCookie =
                httpContext.Request.Cookies[
                    string.IsNullOrWhiteSpace(formsCookieName) ? Guid.NewGuid().ToString() : formsCookieName];
            if (httpCookie == null)
            {
                userData = new UserData();
            }
            else
            {
                FormsAuthenticationTicket authenticationTicket = FormsAuthentication.Decrypt(httpCookie.Value);

                if (!UserData.TryParse(authenticationTicket.UserData, out userData))
                {
                    // No name will mean the Idenity has no name .. which means the user is NOT authenticated. Nice.
                    userData = new UserData();
                }
            }

            var principal = new CustomPrincipal(new CustomIdentity(userData.Id, userData.DisplayName), null);
            httpContext.User = principal;
            Thread.CurrentPrincipal = principal;
        }
    }
}