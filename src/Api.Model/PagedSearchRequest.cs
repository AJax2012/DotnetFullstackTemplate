namespace SourceName.Api.Model
{
    public abstract class PagedSearchRequest : SortedSearchRequest
    {
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}