using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using CuttingEdge.Conditions;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Client.Listeners;
using RavenOverflow.Core.Entities;
using RavenOverflow.FakeData;
using RavenOverflow.Web.Models;
using RavenOverflow.Web.RavenDb;
using RavenOverflow.Web.RavenDb.Indexes;

namespace RavenOverflow.Tests
{
    public abstract class RavenDbFactBase : IDisposable
    {
        private IDocumentStore _documentStore;

        protected IList<Type> IndexesToExecute { get; set; }

        protected bool IsDataToBeSeeded { get; set; }

        protected RavenDbFactBase()
        {
            IsDataToBeSeeded = true;
        }

        protected IDocumentStore DocumentStore
        {
            get
            {
                if (_documentStore != null)
                {
                    return _documentStore;
                }

                var documentStore = new EmbeddableDocumentStore
                                    {
                                        RunInMemory = true
                                    };
                documentStore.InitializeWithDefaults(IsDataToBeSeeded, IndexesToExecute);

                // Force query's to wait for index's to catch up. Unit Testing only :P
                documentStore.RegisterListener(new NoStaleQueriesListener());

                _documentStore = documentStore;

                return _documentStore;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (DocumentStore == null || DocumentStore.WasDisposed)
            {
                return;
            }

            // Assert for any errors.
            DocumentStore.AssertDocumentStoreErrors();

            // Clean up.
            DocumentStore.Dispose();
        }

        #endregion

        #region Nested type: NoStaleQueriesListener

        public class NoStaleQueriesListener : IDocumentQueryListener
        {
            #region IDocumentQueryListener Members

            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
                queryCustomization.WaitForNonStaleResults();
            }

            #endregion
        }

        #endregion
    }
}