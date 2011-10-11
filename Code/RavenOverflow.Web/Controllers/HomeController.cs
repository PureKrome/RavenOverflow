using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Linq;
using RavenOverflow.Core.Entities;
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
        public ActionResult Index(string displayName)
        {
            User user = DocumentSession.Query<User>()
                            .SingleOrDefault(x => x.DisplayName == displayName) ?? new User();

            var viewModel = new IndexViewModel
                                {
                                    // 1. All the questions, ordered by most recent.
                                    Questions = DocumentSession.Query<Question>()
                                        .OrderByDescending(x => x.CreatedOn)
                                        .Take(20)
                                        .ToList(),
                                    // 2. Popular Tags for a time period.
                                    // StackOverflow calls it 'recent tags'.
                                    PopularTagsThisMonth = DocumentSession.Query<RecentTags.ReduceResult, RecentTags>()
                                        .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1))
                                        .Take(20)
                                        .ToList(),
                                    UserTags = user.FavTags,
                                    // 3. Log in user information.
                                    //AuthenticationViewModel = AuthenticationViewModel
                                };

            ViewBag.UserDetails = AuthenticationViewModel;

            return View(viewModel);
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
            var facets = DocumentSession.Query<RecentTagsMapOnly.ReduceResult, RecentTagsMapOnly>()
                .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-1))
                .ToFacets("Raven/Facets/Tags");

            return Json(facets, JsonRequestBehavior.AllowGet);
        }

        [RavenActionFilter]
        public ActionResult TagStats(string id)
        {
            var query = DocumentSession.Query<RecentTags.ReduceResult, RecentTags>()
                .Where(x => x.Tag == id);

            // Does this tag exist?
            var tag = query.FirstOrDefault();

            if (tag != null)
            {
                return Json(tag, JsonRequestBehavior.AllowGet);
            }

            // No exact match .. so lets use Suggest.
            var suggestedTags = query.Suggest();
            if (suggestedTags.Suggestions.Length == 1)
            {
                // We have 1 suggestion, so don't suggest .. just go there :)
                return RedirectToActionPermanent("TagsStats", suggestedTags.Suggestions.First());
            }

            // We have zero or more than 2+ suggestions...
            return Json(new
                            {
                                Error = "Not Found",
                                Message = suggestedTags.Suggestions.Length <= 0
                                              ? "No suggestions found :~("
                                              : "Did you mean?",
                                suggestedTags.Suggestions
                            }, JsonRequestBehavior.AllowGet);
        }
    }
}