using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Services
{
    public class UserService
    {
        private readonly IDocumentSession _documentSession;

        public UserService(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public User CreateOrUpdate(OAuthData oAuthData, string userName, string fullName, string email)
        {
            // First, lets see if we have a user with this id, for this provider.
            User existingUser = _documentSession.Query<User>()
                .Where(
                    x =>
                    x.OAuthData.Any(y => y.Id == oAuthData.Id && y.OAuthProvider == oAuthData.OAuthProvider))
                .SingleOrDefault();

            if (existingUser != null)
            {
                // User exist. All is good :)
                return existingUser;
            }

            // No user exists for the OAuth provider and Id. So lets try their email address.
            existingUser = _documentSession.Query<User>()
                .Where(x => x.Email == email)
                .SingleOrDefault();

            if (existingUser != null)
            {
                // User exist, but isn't associated to this OAuthProvider. So lets do that!
                if (existingUser.OAuthData == null)
                {
                    existingUser.OAuthData = new List<OAuthData>();
                }
                existingUser.OAuthData.Add(oAuthData);
                _documentSession.Store(existingUser);
                _documentSession.SaveChanges();
                return existingUser;
            }

            // Ok. No user at all. We create one and store it.
            var newUser = new User
                              {
                                  DisplayName = userName,
                                  Email = email,
                                  Id = null,
                                  FullName = fullName,
                                  OAuthData = new List<OAuthData>()
                              };
            newUser.OAuthData.Add(oAuthData);

            _documentSession.Store(newUser);
            _documentSession.SaveChanges();

            return newUser;
        }
    }
}