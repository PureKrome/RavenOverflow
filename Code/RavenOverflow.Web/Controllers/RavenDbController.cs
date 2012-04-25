using CuttingEdge.Conditions;
using Raven.Client;

namespace RavenOverflow.Web.Controllers
{
    public abstract class RavenDbController : BaseController
    {
        private readonly IDocumentStore _documentStore;
        private IDocumentSession _documentSession;

        protected RavenDbController(IDocumentStore documentStore)
        {
            Condition.Requires(documentStore).IsNotNull();

            _documentStore = documentStore;
        }

        public IDocumentSession DocumentSession
        {
            get { return _documentSession ?? (_documentSession = _documentStore.OpenSession()); }
        }
    }
}