using System.Collections.Generic;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Web.Models.ViewModels
{
    public class IndexViewModel : _LayoutViewModel
    {
        public IndexViewModel(ICustomIdentity customIdentity) : base(customIdentity)
        {
        }

        public QuestionListViewModel QuestionListViewModel { get; set; }
        public AuthenticationViewModel AuthenticationViewModel { get; set; }
        public IDictionary<string, short> RecentPopularTags { get; set; }
        public UserTagListViewModel UserFavoriteTagListViewModel { get; set; }
        public UserTagListViewModel UserIgnoredTagList { get; set; }
    }
}