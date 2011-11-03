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
using RavenOverflow.Web.Models;
using RavenOverflow.Web.Models.ViewModels;

namespace RavenOverflow.Web.Controllers
{
    public class HomeController : AbstractController
    {
        public HomeController(IDocumentSession documentSession): base(documentSession)
        {
        }

        //[HttpGet, RavenActionFilter]
        public ActionResult Index(string displayName, string tag)
        {
            string header = "Top Questions";

            // 1. All the questions, ordered by most recent.
            IQueryable<Question> questionsQuery = DocumentSession.Query<Question>()
                .OrderByDescending(x => x.CreatedOn)
                .Take(20);

            // Filter Questions by Tags?
            if (!string.IsNullOrEmpty(tag))
            {
                header = "Tagged Questions";
                questionsQuery = questionsQuery
                    .Where(x => x.Tags.Any(y => y == tag));
            }

            // 2. Popular Tags for a time period.
            // StackOverflow calls it 'recent tags'.
            IQueryable<RecentPopularTags.ReduceResult> recentPopularTags =
                DocumentSession.Query<RecentPopularTags.ReduceResult, RecentPopularTags>()
                    .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1).ToUtcToday())
                    .Take(20);

            // 3. Log in user information.
            var name = displayName ?? User.Identity.Name;
            IRavenQueryable<User> userQuery = DocumentSession.Query<User>()
                .Where(x => x.DisplayName == (name));

            var viewModel = new IndexViewModel(User.Identity)
                                {
                                    Header = header,
                                    Questions = questionsQuery.ToList(),
                                    RecentPopularTags = recentPopularTags.ToDictionary(x => x.Tag, x => x.Count),
                                    UserTags = (userQuery.SingleOrDefault() ?? new User()).FavTags
                                };

            return View(viewModel);
        }

        //[HttpGet, RavenActionFilter]
        public ActionResult BatchedIndex(string displayName)
        {
            // 1. All the questions, ordered by most recent.
            Lazy<IEnumerable<Question>> questionsQuery = DocumentSession.Query<Question>()
                .OrderByDescending(x => x.CreatedOn)
                .Take(20)
                .Lazily();

            // 2. Popular Tags for a time period.
            // StackOverflow calls it 'recent tags'.
            Lazy<IEnumerable<RecentPopularTags.ReduceResult>> recentPopularTags =
                DocumentSession.Query<RecentPopularTags.ReduceResult, RecentPopularTags>()
                    .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1).ToUtcToday())
                    .Take(20)
                    .Lazily();

            // 3. Log in user information.
            //AuthenticationViewData = AuthenticationViewData
            Lazy<IEnumerable<User>> userQuery = DocumentSession.Query<User>()
                .Where(x => x.DisplayName == displayName)
                .Lazily();

            var viewModel = new IndexViewModel(User.Identity)
                                {
                                    Header = "Top Questions",
                                    Questions = questionsQuery.Value.ToList(),
                                    RecentPopularTags = recentPopularTags.Value.ToDictionary(x => x.Tag, x => x.Count),
                                    UserTags = (userQuery.Value.SingleOrDefault() ?? new User()).FavTags
                                };

            return View("Index", viewModel);
        }

        //[HttpGet, RavenActionFilter]
        public ActionResult AggressiveIndex(string displayName)
        {
            using (DocumentSession.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(1)))
            {
                // 1. All the questions, ordered by most recent.
                Lazy<IEnumerable<Question>> questionsQuery = DocumentSession.Query<Question>()
                    .OrderByDescending(x => x.CreatedOn)
                    .Take(20)
                    .Lazily();

                // 2. Popular Tags for a time period.
                // StackOverflow calls it 'recent tags'.
                Lazy<IEnumerable<RecentPopularTags.ReduceResult>> recentPopularTags =
                    DocumentSession.Query<RecentPopularTags.ReduceResult, RecentPopularTags>()
                        .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1).ToUtcToday())
                        .Take(20)
                        .Lazily();

                // 3. Log in user information.
                //AuthenticationViewData = AuthenticationViewData
                Lazy<IEnumerable<User>> userQuery = DocumentSession.Query<User>()
                    .Where(x => x.DisplayName == displayName)
                    .Lazily();

                var viewModel = new IndexViewModel(User.Identity)
                                    {
                                        Header = "Top Questions",
                                        Questions = questionsQuery.Value.ToList(),
                                        RecentPopularTags = recentPopularTags.Value.ToDictionary(x => x.Tag, x => x.Count),
                                        UserTags = (userQuery.Value.SingleOrDefault() ?? new User()).FavTags
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
                .OrderByDescending(x => x.CreatedOn)
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
    }
}