using System;
using System.Linq;
using Raven.Client.Indexes;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Web.RavenDb.Indexes
{
    public class RecentPopularTagsMapOnly : AbstractIndexCreationTask<Question, RecentPopularTagsMapOnly.ReduceResult>
    {
        public RecentPopularTagsMapOnly()
        {
            Map = questions => from question in questions
                               from tag in question.Tags
                               select new
                                          {
                                              Tag = tag,
                                              LastSeen = (DateTimeOffset) question.CreatedOn
                                          };
        }

        #region Nested type: ReduceResult

        public class ReduceResult
        {
            public string Tag { get; set; }
            public int Count { get; set; }
            public DateTimeOffset LastSeen { get; set; }
        }

        #endregion
    }
}