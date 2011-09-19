using System.Linq;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Core.Services
{
    public interface IUserService
    {
        IQueryable<User> Query();
        void Store(User user);
    }
}