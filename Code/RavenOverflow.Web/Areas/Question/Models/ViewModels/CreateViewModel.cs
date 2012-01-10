using RavenOverflow.Web.Models;
using RavenOverflow.Web.Models.Authentication;

namespace RavenOverflow.Web.Areas.Question.Models.ViewModels
{
    public class CreateViewModel : _LayoutViewModel
    {
        public CreateViewModel(ICustomIdentity customIdentity) : base(customIdentity)
        {
        }
    }
}