using System.Collections.Generic;

namespace RavenOverflow.Web.Models.ViewModels
{
    public class QuestionListViewModel
    {
        public IList<QuestionWithDisplayName> Questions { get; set; }
    }
}