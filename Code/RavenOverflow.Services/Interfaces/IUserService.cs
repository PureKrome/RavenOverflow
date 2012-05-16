using RavenOverflow.Core.Entities;

namespace RavenOverflow.Services.Interfaces
{
    public interface IUserService
    {
        User CreateOrUpdate(OAuthData oAuthData, string userName, string fullName, string email);
    }
}