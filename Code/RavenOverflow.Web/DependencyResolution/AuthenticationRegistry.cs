using DotNetOpenAuth.OAuth.ChannelElements;
using RavenOverflow.Web.Models.OAuth;
using StructureMap.Configuration.DSL;

namespace RavenOverflow.Web.DependencyResolution
{
    public class AuthenticationRegistry : Registry
    {
        public AuthenticationRegistry(string twitterConsumerKey, string twitterConsumerSecret)
        {
            For<IConsumerTokenManager>()
                .Use<RavenDbConsumerTokenManager>()
                .Ctor<string>("consumerKey").Is(twitterConsumerKey)
                .Ctor<string>("consumerSecret").Is(twitterConsumerSecret)
                .Named("RavenDb OAuth Consumer Token Manager.");
        }
    }
}