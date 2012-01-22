using RavenOverflow.Core.Entities;

namespace RavenOverflow.Web.Models.ViewModels
{
    public class QuestionWithDisplayName : Question
    {
        public string DisplayName { get; set; }
    }
}