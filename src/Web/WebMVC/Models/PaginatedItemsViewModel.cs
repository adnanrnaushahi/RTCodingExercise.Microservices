
namespace WebMVC.ViewModels
{
    public class PaginatedItemsViewModel<T> where T : class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Data { get; set; }
        public bool HasPreviousPage => PageIndex > 0;
        public bool HasNextPage => PageIndex + 1 < TotalPages;
    }
}
