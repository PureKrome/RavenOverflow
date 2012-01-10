using System.Web.Mvc;

namespace RavenOverflow.Web.Areas.Question
{
    public class QuestionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Question"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Question - ask",
                "questions/ask",
                new { controller = "Questions", action = "Create" }
                );

            context.MapRoute(
                "Question_default",
                "questions/{action}/{id}",
                new { controller = "Questions", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}