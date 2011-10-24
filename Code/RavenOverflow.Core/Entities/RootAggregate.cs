using Newtonsoft.Json;

namespace RavenOverflow.Core.Entities
{
    public abstract class RootAggregate
    {
        public string Id { get; set; }

        [JsonIgnore]
        public int IdAsANumber
        {
            get
            {
                if (string.IsNullOrEmpty(Id))
                {
                    return 0;
                }

                int index = Id.IndexOf('/');
                if (index < 0)
                {
                    return 0;
                }

                int id;
                return int.TryParse(Id.Substring(index + 1, Id.Length - index - 1), out id) ? id : 0;
            }
        }
    }
}