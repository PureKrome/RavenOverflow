using System;
using System.Linq;
using Raven.Client.Indexes;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.Web.Indexes
{
    public class RecentTags : AbstractIndexCreationTask<Question, RecentTags.ReduceResult>
    {
        public RecentTags()
        {
            Map = questions => from question in questions
                               from tag in question.Tags
                               select new
                                          {
                                              Tag = tag,
                                              Count = 1,
                                              LastSeen = (DateTimeOffset) question.CreatedOn
                                          };

            Reduce = results => from result in results
                                group result by result.Tag
                                into g
                                select new
                                           {
                                               Tag = g.Key,
                                               Count = g.Sum(x => x.Count),
                                               LastSeen = g.Max(x => x.LastSeen)
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