namespace RavenOverflow.Web.Models.Authentication
{
    public interface IOAuthAuthentication
    {
        string FacebookAppId { get; }
        string FacebookSecret { get; }
    }
}