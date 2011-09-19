using System.Linq;
using CuttingEdge.Conditions;
using Raven.Client;
using RavenOverflow.Core.Entities;
using RavenOverflow.Core.Services;

namespace RavenOverflow.Services
{
    public class UserService : IUserService
    {
        private readonly IDocumentSession _documentSession;

        public UserService(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        #region Implementation of IUserService

        public IQueryable<User> Query()
        {
            return _documentSession.Query<User>();
        }

        public void Store(User user)
        {
            Condition.Requires(user).IsNotNull();

            _documentSession.Store(user);
        }

        #endregion
    }
}