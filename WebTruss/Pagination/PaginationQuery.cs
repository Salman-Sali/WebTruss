using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebTruss.Pagination
{
    public class PaginationQuery
    {
        [DefaultValue(PAGE_DEFAULT)]
        [Required]
        public int Page { get; set; } = PAGE_DEFAULT;


        [DefaultValue(LIMIT_DEFAULT)]
        [Required]
        public int Limit { get; set; } = LIMIT_DEFAULT;


        [DefaultValue(SORTBY_DEFAULT)]
        [Required]
        public string SortBy { get; set; } = SORTBY_DEFAULT;

        [DefaultValue(SORT_BY_ASCENDING_DEFAULT)]
        [Required]
        public bool SortByAscending { get; set; } = SORT_BY_ASCENDING_DEFAULT;
    }

    public struct PagedQueryConstants
    {
        public const int PAGE_DEFAULT = 0;
        public const int LIMIT_DEFAULT = 0;
        public const string SORTBY_DEFAULT = "Id";
        public const bool SORT_BY_ASCENDING_DEFAULT = true;
    }
}
