using System;
using System.Linq;
using Raven.Client.Indexes;
using Raven.Client.Linq;
using RavenOverflow.Core.Entities;
using RavenOverflow.Core.Extensions;

namespace RavenOverflow.Web.Indexes
{
	public class RecentPopularTags : AbstractIndexCreationTask<Question, RecentPopularTags.ReduceResult>
	{
		public RecentPopularTags()
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
											LastSeen = g.Max(x => (DateTimeOffset)x.LastSeen)
			                           	};

			SortOptions.Add(x => x.Count, Raven.Abstractions.Indexing.SortOptions.Short);
		}

		#region Nested type: ReduceResult

		public class ReduceResult
		{
			public string Tag { get; set; }
			public short Count { get; set; }
			public DateTimeOffset LastSeen { get; set; }

			public override string ToString()
			{
				return string.Format("{0} : {1} : {2}", Tag, Count, LastSeen);
			}
		}

		#endregion
	}

	#region Extensions

	public static class Extensions
	{
		public static IRavenQueryable<RecentPopularTags.ReduceResult> WithinTheLastMonth(
			this IRavenQueryable<RecentPopularTags.ReduceResult> query, int numberOfMonths)
		{
			return query.Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(numberOfMonths*(-1)).ToUtcToday());
		}

		public static IRavenQueryable<RecentPopularTags.ReduceResult> OrderByCountDescending(
			this IRavenQueryable<RecentPopularTags.ReduceResult> query)
		{
			return query.OrderByDescending(x => x.Count);
		}
	}

	#endregion
}