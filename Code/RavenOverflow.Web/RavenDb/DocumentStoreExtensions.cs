using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using CuttingEdge.Conditions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using RavenOverflow.Web.RavenDb.Indexes;

namespace RavenOverflow.Web.RavenDb
{
    public static class DocumentStoreExtensions
    {
        public static void InitializeWithDefaults(this IDocumentStore documentStore)
        {
            documentStore.InitializeWithDefaults(true, null);
        }

        public static void InitializeWithDefaults(this IDocumentStore documentStore, bool isDataToBeSeeded,
                                                  IList<Type> indexesToExecute)
        {
            Condition.Requires(documentStore).IsNotNull();

            // Default initializtion;
            documentStore.Initialize();

            // Index initialisation.
            if (indexesToExecute != null)
            {
                Type[] indexes = (from type in indexesToExecute
                                  where type.IsSubclassOf(typeof (AbstractIndexCreationTask))
                                  select type).ToArray();

                IndexCreation.CreateIndexes(new CompositionContainer(new TypeCatalog(indexes)), documentStore);
            }
            else
            {
                IndexCreation.CreateIndexes(typeof (RecentPopularTags).Assembly, documentStore);
            }

            // Create our Seed Data (if required).
            // NOTE: This would be handled differently if it was a -real- production system.
            //       Like, wrapping this called in a #if RELEASE #endif, for example.
            if (isDataToBeSeeded)
            {
                HelperUtilities.CreateSeedData(documentStore);
            }

            // Now lets check to make sure there are now errors.
            documentStore.AssertDocumentStoreErrors();
        }

        public static void AssertDocumentStoreErrors(this IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException("documentStore");
            }

            ServerError[] errors = documentStore.DatabaseCommands.GetStatistics().Errors;
            if (errors == null || errors.Length <= 0)
            {
                return;
            }

            // We have some Errors. NOT. GOOD. :(
            var errorMessages = new StringBuilder();
            foreach (ServerError serverError in errors)
            {
                string errorMessage = string.Format("Document: {0}; Index: {1}; Error: {2}",
                                                    string.IsNullOrEmpty(serverError.Document)
                                                        ? "No Document Id"
                                                        : serverError.Document,
                                                    string.IsNullOrEmpty(serverError.Index)
                                                        ? "No Index"
                                                        : serverError.Index,
                                                    string.IsNullOrEmpty(serverError.Error)
                                                        ? "No Error message .. err??"
                                                        : serverError.Error);


                Debug.WriteLine(errorMessage);
                errorMessages.AppendLine(errorMessage);
            }

            throw new InvalidOperationException("DocumentStore has some errors. Dast is nict gut.");
        }

        public static void WaitForStaleIndexesToComplete(this IDocumentStore documentStore)
        {
            while (documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0)
            {
                Thread.Sleep(50);
            }
        }
    }
}