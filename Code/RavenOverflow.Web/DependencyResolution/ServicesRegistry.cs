using CuttingEdge.Conditions;
using RavenOverflow.Services;
using StructureMap.Configuration.DSL;

namespace RavenOverflow.Web.DependencyResolution
{
    public class ServicesRegistry : Registry
    {
        public ServicesRegistry(string facebookAppId, string facebookAppSecret)
        {
            Condition.Requires(facebookAppId).IsNullOrEmpty();
            Condition.Requires(facebookAppSecret).IsNotNullOrEmpty();

        }
    }
}