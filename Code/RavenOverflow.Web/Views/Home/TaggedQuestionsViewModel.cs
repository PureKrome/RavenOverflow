using System.Collections.Generic;
using RavenOverflow.Core.Entities;
using RavenOverflow.Web.Views.Shared;

namespace RavenOverflow.Web.Views.Home
{
    public class TaggedQuestionsViewModel
    {
        public IList<Question> Questions { get; set; }
        public AuthenticationViewModel AuthenticationViewModel { get; set; }
    }
}