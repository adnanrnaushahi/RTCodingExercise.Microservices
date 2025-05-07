namespace Catalog.API.DTO
{
    public class PaginatedItemsDto<T> where T : class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long Count { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
