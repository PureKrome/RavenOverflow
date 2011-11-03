using System.Collections.Generic;

namespace RavenOverflow.Web.Models.ViewModels
{
    public class UserTagListViewModel
    {
        public string Header { get; set; }
        public string DivId1 { get; set; }
        public string DivId2 { get; set; }
        public IList<string> Tags { get; set; }
    }
}