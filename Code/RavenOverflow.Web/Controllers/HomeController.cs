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
using RavenOverflow.Web.Views.Home;

namespace RavenOverflow.Web.Controllers
{
    public class HomeController : AbstractController
    {
        public HomeController(IDocumentStore documentStore) : base(documentStore)
        {
        }

        [RavenActionFilter]
        public ActionResult Index(string displayName, string tag)
        {
            string header = "Top Questions";

            // 1. All the questions, ordered by most recent.
            IQueryable<Question> questionsQuery = DocumentSession.Query<Question>()
                .OrderByDescending(x => x.CreatedOn)
                .Take(20);

            // Filter by Tags?
            if (!string.IsNullOrEmpty(tag))
            {
                header = "Tagged Questions";
                questionsQuery = questionsQuery
                    .Where(x => x.Tags.Any(y => y == tag));
            }

            // 2. Popular Tags for a time period.
            // StackOverflow calls it 'recent tags'.
            IQueryable<RecentTags.ReduceResult> popularTagsThisMonthQuery =
                DocumentSession.Query<RecentTags.ReduceResult, RecentTags>()
                    .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1).ToUtcToday())
                    .Take(20);

            // 3. Log in user information.
            //AuthenticationViewModel = AuthenticationViewModel
            IRavenQueryable<User> userQuery = DocumentSession.Query<User>()
                .Where(x => x.DisplayName == displayName);

            var viewModel = new IndexViewModel
                                {
                                    Header = header,
                                    Questions = questionsQuery.ToList(),
                                    PopularTagsThisMonth = popularTagsThisMonthQuery.ToList(),
                                    UserTags = (userQuery.SingleOrDefault() ?? new User()).FavTags
                                };

            ViewBag.UserDetails = AuthenticationViewModel;

            return View(viewModel);
        }

        [RavenActionFilter]
        public ActionResult BatchedIndex(string displayName)
        {
            // 1. All the questions, ordered by most recent.
            Lazy<IEnumerable<Question>> questionsQuery = DocumentSession.Query<Question>()
                .OrderByDescending(x => x.CreatedOn)
                .Take(20)
                .Lazily();

            // 2. Popular Tags for a time period.
            // StackOverflow calls it 'recent tags'.
            Lazy<IEnumerable<RecentTags.ReduceResult>> popularTagsThisMonthQuery =
                DocumentSession.Query<RecentTags.ReduceResult, RecentTags>()
                    .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1).ToUtcToday())
                    .Take(20)
                    .Lazily();

            // 3. Log in user information.
            //AuthenticationViewModel = AuthenticationViewModel
            Lazy<IEnumerable<User>> userQuery = DocumentSession.Query<User>()
                .Where(x => x.DisplayName == displayName)
                .Lazily();

            var viewModel = new IndexViewModel
                                {
                                    Header = "Top Questions",
                                    Questions = questionsQuery.Value.ToList(),
                                    PopularTagsThisMonth = popularTagsThisMonthQuery.Value.ToList(),
                                    UserTags = (userQuery.Value.SingleOrDefault() ?? new User()).FavTags
                                };

            ViewBag.UserDetails = AuthenticationViewModel;

            return View("Index", viewModel);
        }

        [RavenActionFilter]
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
                Lazy<IEnumerable<RecentTags.ReduceResult>> popularTagsThisMonthQuery =
                    DocumentSession.Query<RecentTags.ReduceResult, RecentTags>()
                        .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1).ToUtcToday())
                        .Take(20)
                        .Lazily();

                // 3. Log in user information.
                //AuthenticationViewModel = AuthenticationViewModel
                Lazy<IEnumerable<User>> userQuery = DocumentSession.Query<User>()
                    .Where(x => x.DisplayName == displayName)
                    .Lazily();

                var viewModel = new IndexViewModel
                                    {
                                        Header = "Top Questions",
                                        Questions = questionsQuery.Value.ToList(),
                                        PopularTagsThisMonth = popularTagsThisMonthQuery.Value.ToList(),
                                        UserTags = (userQuery.Value.SingleOrDefault() ?? new User()).FavTags
                                    };

                ViewBag.UserDetails = AuthenticationViewModel;

                return View("Index", viewModel);
            }
        }

        [RavenActionFilter]
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

        [RavenActionFilter]
        public ActionResult Facets(string id)
        {
            IDictionary<string, IEnumerable<FacetValue>> facets = DocumentSession.Query
                <RecentTagsMapOnly.ReduceResult, RecentTagsMapOnly>()
                .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1).ToUtcToday())
                .ToFacets("Raven/Facets/Tags");

            return Json(facets, JsonRequestBehavior.AllowGet);
        }

        [RavenActionFilter]
        public ActionResult Search(string term)
        {
            IRavenQueryable<RecentTags.ReduceResult> query = DocumentSession
                .Query<RecentTags.ReduceResult, RecentTags>()
                .Where(x => x.Tag == term);

            // Does this tag exist?
            RecentTags.ReduceResult tag = query.FirstOrDefault();

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