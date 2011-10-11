using System.Collections.Generic;
using RavenOverflow.Core.Entities;
using RavenOverflow.Web.Indexes;
using RavenOverflow.Web.Views.Shared;

namespace RavenOverflow.Web.Views.Home
{
    public class IndexViewModel
    {
        public IList<Question> Questions { get; set; }
        public AuthenticationViewModel AuthenticationViewModel { get; set; }
        public IList<RecentTags.ReduceResult> PopularTagsThisMonth { get; set; }
        public IList<string> UserTags { get; set; }
    }
}