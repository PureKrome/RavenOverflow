namespace RavenOverflow.Web.Models
{
    public static class HtmlExtensions
    {
        public static string ToSimplifiedNumberText(this int value)
        {
            return value < 1000 ? value.ToString() : string.Format("{0}k", value/1000);
        }
    }
}