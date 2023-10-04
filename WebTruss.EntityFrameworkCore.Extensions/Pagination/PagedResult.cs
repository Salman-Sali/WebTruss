namespace WebTruss.EntityFrameworkCore.Extensions.Pagination
{
    public class PagedResult<TModel>
    {
        public int Page { get; }
        public int Limit { get; }
        public int TotalItems { get; }
        public int TotalPages { get; }
        public IList<TModel> Items { get; }

        public PagedResult(int page, int limit, int totalItems, int totalPages, IEnumerable<TModel> items)
        {
            Page = page;
            Limit = limit;
            TotalItems = totalItems;
            TotalPages = totalPages;
            Items = items.ToList();
        }
    }
}
