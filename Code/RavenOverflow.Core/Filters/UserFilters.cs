using System;
using System.Linq;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Core.Filters
{
    public static class UserFilters
    {
        public static IQueryable<User> WithDisplayName(this IQueryable<User> query, string displayName)
        {
            if(string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentNullException("displayName");
            }

            return query.Where(x => x.DisplayName == displayName);
        }
    }
}