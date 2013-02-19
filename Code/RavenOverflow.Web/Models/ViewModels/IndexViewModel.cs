using System.Collections.Generic;

namespace RavenOverflow.Web.Models.ViewModels
{
    public class IndexViewModel : _LayoutViewModel
    {
        public IndexViewModel(ClaimsUser claimsUser) : base(claimsUser)
        {
        }

        public QuestionListViewModel QuestionListViewModel { get; set; }
        public IDictionary<string, short> RecentPopularTags { get; set; }
        public UserTagListViewModel UserFavoriteTagListViewModel { get; set; }
        public UserTagListViewModel UserIgnoredTagList { get; set; }
    }
}