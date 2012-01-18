using System;

namespace RavenOverflow.Web.Models.Authentication
{
    public class UserData
    {
        private const char Delimeter = ',';

        public string Id { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return string.Format("{1}{0}{2}", Delimeter, Id, string.IsNullOrEmpty(DisplayName) ? "-" : DisplayName);
        }

        public static bool TryParse(string data, out UserData userData)
        {
            if (string.IsNullOrWhiteSpace("data"))
            {
                throw new ArgumentNullException("data");
            }

            userData = null;

            // Split the text into segments.
            string[] segments = data.Split(new[] { Delimeter }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length != 2)
            {
                return false;
            }

            userData = new UserData
            {
                Id = segments[0],
                DisplayName = segments[1]
            };

            return true;
        }
    }
}