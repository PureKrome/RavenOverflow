using RavenOverflow.Core.Entities;

namespace RavenOverflow.Core.Services
{
    public interface IUserService
    {
        User CreateOrUpdate(OAuthData oAuthData, string userName, string fullName, string email);
    }
}