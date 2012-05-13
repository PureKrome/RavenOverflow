using System;
using System.Linq;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Core.Filters
{
    public static class QuestionFilters
    {
        public static IQueryable<Question> OrderByCreatedByDescending(this IQueryable<Question> query)
        {
            return query.OrderByDescending(x => x.CreatedOn);
        }

        public static IQueryable<Question> WithAnyTag(this IQueryable<Question> query, string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }

            return query.Where(x => x.Tags.Any(y => y == tag));
        }
    }
}