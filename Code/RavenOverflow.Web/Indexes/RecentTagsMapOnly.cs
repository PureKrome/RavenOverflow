using System;
using System.Linq;
using Raven.Client.Indexes;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Web.Indexes
{
    public class RecentTagsMapOnly : AbstractIndexCreationTask<Question, RecentTagsMapOnly.ReduceResult>
    {
        public RecentTagsMapOnly()
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