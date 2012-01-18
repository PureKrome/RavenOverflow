using System.Web;

namespace RavenOverflow.Web.Models.Authentication
{
    public interface ICustomFormsAuthentication
    {
        void SignIn(string id, string displayName, HttpResponseBase httpResponseBase);
        void SignOut();
    }
}