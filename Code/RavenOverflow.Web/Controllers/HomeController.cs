using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using RavenOverflow.Core.Entities;
using RavenOverflow.Core.Extensions;
using RavenOverflow.Web.Indexes;
using RavenOverflow.Web.Models.ViewModels;

namespace RavenOverflow.Web.Controllers
{
    public class HomeController : AbstractController
    {
        public HomeController(IDocumentSession documentSession) : base(documentSession)
        {
        }

        //[HttpGet, RavenActionFilter]
        public ActionResult Index(string displayName, string tag)
        {
            string header;

            // 1. All the questions, ordered by most recent.
            var questionsQuery = QuestionQuery(tag, out header);

            // 2. Popular Tags for a time period.
            // StackOverflow calls it 'recent tags'.
            var recentPopularTags = RecentPopularTagsQuery();

            // 3. Log in user information.
            var userQuery = UserQuery(displayName);

            var viewModel = new IndexViewModel(User.Identity)
                                {
                                    Header = header,
                                    Questions = questionsQuery.ToList(),
                                    RecentPopularTags = recentPopularTags.ToDictionary(x => x.Tag, x => x.Count),
                                    UserFavoriteTagList = new UserTagListViewModel
                                                              {
                                                                  Header = "Favorite Tags",
                                                                  DivId1 = "interesting-tags",
                                                                  DivId2 = "interestingtags",
                                                                  Tags = userQuery == null ? null :
                                                                      (userQuery.SingleOrDefault() ?? new User()).FavoriteTags
                                                              },
                                    UserIgnoredTagList = new UserTagListViewModel
                                                             {
                                                                 Header = "Ignored Tags",
                                                                 DivId1 = "ignored-tags",
                                                                 DivId2 = "ignoredtags",
                                                                 Tags = null
                                                             }
                                };

            return View(viewModel);
        }

        //[HttpGet, RavenActionFilter]
        public ActionResult BatchedIndex(string displayName, string tag)
        {
            string header;

            // 1. All the questions, ordered by most recent.
            var questionsQuery = QuestionQuery(tag, out header).Lazily();

            // 2. Popular Tags for a time period.
            // StackOverflow calls it 'recent tags'.
            var recentPopularTags = RecentPopularTagsQuery().Lazily();

            // 3. Log in user information.
            var userQuery = UserQuery(displayName);
            var lazyUserQuery = (userQuery != null ? userQuery.Lazily() : null);

            var viewModel = new IndexViewModel(User.Identity)
                                {
                                    Header = header,
                                    Questions = questionsQuery.Value.ToList(),
                                    RecentPopularTags = recentPopularTags.Value.ToDictionary(x => x.Tag, x => x.Count),
                                    UserFavoriteTagList = new UserTagListViewModel
                                                              {
                                                                  Header = "Favorite Tags",
                                                                  DivId1 = "interesting-tags",
                                                                  DivId2 = "interestingtags",
                                                                  Tags = lazyUserQuery == null ? null :
                                                                      (lazyUserQuery.Value.SingleOrDefault() ?? new User()).FavoriteTags
                                                              },
                                    UserIgnoredTagList = new UserTagListViewModel
                                                             {
                                                                 Header = "Ignored Tags",
                                                                 DivId1 = "ignored-tags",
                                                                 DivId2 = "ignoredtags",
                                                                 Tags = null
                                                             }
                                };

            return View("Index", viewModel);
        }

        //[HttpGet, RavenActionFilter]
        public ActionResult AggressiveIndex(string displayName, string tag)
        {
            using (DocumentSession.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(1)))
            {
                string header;

                // 1. All the questions, ordered by most recent.
                var questionsQuery = QuestionQuery(tag, out header).Lazily();

                // 2. Popular Tags for a time period.
                // StackOverflow calls it 'recent tags'.
                var recentPopularTags = RecentPopularTagsQuery().Lazily();

                // 3. Log in user information.
                var userQuery = UserQuery(displayName);
                var lazyUserQuery = (userQuery != null ? userQuery.Lazily() : null);

                var viewModel = new IndexViewModel(User.Identity)
                                    {
                                        Header = header,
                                        Questions = questionsQuery.Value.ToList(),
                                        RecentPopularTags =
                                            recentPopularTags.Value.ToDictionary(x => x.Tag, x => x.Count),
                                        UserFavoriteTagList = new UserTagListViewModel
                                                                  {
                                                                      Header = "Favorite Tags",
                                                                      DivId1 = "interesting-tags",
                                                                      DivId2 = "interestingtags",
                                                                      Tags = lazyUserQuery == null ? null :
    (lazyUserQuery.Value.SingleOrDefault() ?? new User()).FavoriteTags
                                                                  },
                                        UserIgnoredTagList = new UserTagListViewModel
                                                                 {
                                                                     Header = "Ignored Tags",
                                                                     DivId1 = "ignored-tags",
                                                                     DivId2 = "ignoredtags",
                                                                     Tags = null
                                                                 }
                                    };

                return View("Index", viewModel);
            }
        }

        //[RavenActionFilter]
        public ActionResult Tag(string id)
        {
            RavenQueryStatistics stats;
            List<Question> questions = DocumentSession.Query<Question>()
                .Statistics(out stats)
                .OrderByCreatedByDescending()
                .Take(20)
                .Where(x => x.Tags.Any(tag => tag == id))
                .ToList();

            return Json(new
                            {
                                Questions = questions,
                                stats.TotalResults,
                                Tag = DocumentSession.Load<Tag>("tags/" + id)
                            }, JsonRequestBehavior.AllowGet);
        }

        //[RavenActionFilter]
        public ActionResult Facets(string id)
        {
            IDictionary<string, IEnumerable<FacetValue>> facets = DocumentSession.Query
                <RecentPopularTagsMapOnly.ReduceResult, RecentPopularTagsMapOnly>()
                .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1).ToUtcToday())
                .ToFacets("Raven/Facets/Tags");

            return Json(facets, JsonRequestBehavior.AllowGet);
        }

        //[RavenActionFilter]
        public ActionResult Search(string term)
        {
            IRavenQueryable<RecentPopularTags.ReduceResult> query = DocumentSession
                .Query<RecentPopularTags.ReduceResult, RecentPopularTags>()
                .Where(x => x.Tag == term);

            // Does this tag exist?
            RecentPopularTags.ReduceResult tag = query.FirstOrDefault();

            var results = new List<string>();

            if (tag != null)
            {
                results.Add(tag.Tag);
            }
            else
            {
                // No exact match .. so lets use Suggest.
                SuggestionQueryResult suggestedTags = query.Suggest();
                if (suggestedTags.Suggestions.Length == 1)
                {
                    // We have 1 suggestion, so don't suggest .. just go there :)
                    results.Add(suggestedTags.Suggestions.First());
                }
                else
                {
                    // We have zero or more than 2+ suggestions...
                    results.AddRange(suggestedTags.Suggestions);
                }
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        private IQueryable<Question> QuestionQuery(string tag, out string header)
        {
            header = "Top Questions";

            IQueryable<Question> questionsQuery = DocumentSession.Query<Question>()
                .OrderByCreatedByDescending()
                .Take(20);

            // Filter Questions by Tags?
            if (!string.IsNullOrEmpty(tag))
            {
                header = "Tagged Questions";
                questionsQuery = questionsQuery
                    .WithAnyTag(tag);
            }

            return questionsQuery;
        }

        private IQueryable<RecentPopularTags.ReduceResult> RecentPopularTagsQuery()
        {
            IQueryable<RecentPopularTags.ReduceResult> recentPopularTags =
                DocumentSession.Query<RecentPopularTags.ReduceResult, RecentPopularTags>()
                    .WithinTheLastMonth(1)
                    .OrderByCountDescending()
                    .Take(20);

            return recentPopularTags;
        }

        private IQueryable<User> UserQuery(string displayName)
        {
            string name = displayName ?? User.Identity.Name;
            return !string.IsNullOrEmpty(name) ? DocumentSession.Query<User>().WithDisplayName(name) : null;
        }
    }
}