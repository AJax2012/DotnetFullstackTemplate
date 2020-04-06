using System.Collections.Generic;

namespace SourceName.Api.Model
{
    public class SearchResultResource<TResource>
    {
        public int TotalCount { get; set; }
        public List<TResource> Results { get; set; }
    }
}