using System.Collections.Generic;

namespace RavenOverflow.Web.Models.ViewModels
{
    public class TagListViewModel
    {
        public ICollection<string> Tags { get; set; }
        public bool IsUserTags { get; set; }
    }
}