using CuttingEdge.Conditions;
using Raven.Client;

namespace RavenOverflow.Services
{
    public abstract class RavenDbBaseService
    {
        protected RavenDbBaseService(IDocumentSession documentSession)
        {
            Condition.Requires(documentSession).IsNotNull();

            DocumentSession = documentSession;
        }

        public IDocumentSession DocumentSession { get; private set; }
    }
}