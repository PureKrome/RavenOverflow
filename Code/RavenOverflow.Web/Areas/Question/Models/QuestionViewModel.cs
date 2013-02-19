using System.Security.Principal;
using RavenOverflow.Web.Models;

namespace RavenOverflow.Web.Areas.Question.Models
{
    public class QuestionViewModel : _LayoutViewModel
    {
        public QuestionViewModel(ClaimsUser claimsUser) : base(claimsUser)
        {
        }

        public string Subject { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
    }
}