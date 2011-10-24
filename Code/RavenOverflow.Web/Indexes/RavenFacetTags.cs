using System.Collections.Generic;
using Raven.Abstractions.Data;
using Raven.Client;

namespace RavenOverflow.Web.Indexes
{
    public static class RavenFacetTags
    {
        public static void CreateFacets(IDocumentStore documentStore)
        {
            using (IDocumentSession session = documentStore.OpenSession())
            {
                session.Store(new FacetSetup
                                  {
                                      Id = "Raven/Facets/Tags",
                                      Facets = new List<Facet>
                                                   {
                                                       new Facet
                                                           {
                                                               Mode = FacetMode.Default,
                                                               Name = "Tag"
                                                           }
                                                   }
                                  });
                session.SaveChanges();
            }
        }
    }
}