namespace WebTruss.DynamoDB.Pagination
{
    public class PagedDynamoResult<T>
    {
        public string? PaginationToken { get; set; }
        public List<T> Items { get; set; }
    }
}
