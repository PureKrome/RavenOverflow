using System.Collections.Generic;
using RavenOverflow.Core.Entities;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Web.Models.ViewModels
{
    public class IndexViewModel : _LayoutViewModel
    {
        public IndexViewModel(ICustomIdentity customIdentity) : base(customIdentity)
        {
        }

        public IList<Question> Questions { get; set; }
        public AuthenticationViewModel AuthenticationViewModel { get; set; }
        public IDictionary<string, int> RecentPopularTags { get; set; }
        public UserTagListViewModel UserFavoriteTagList { get; set; }
        public UserTagListViewModel UserIgnoredTagList { get; set; }
    }
}