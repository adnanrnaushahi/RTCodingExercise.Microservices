namespace WebMVC.ViewModels
{
    public class SearchViewModel
    {
        public string? SearchTerm { get; set; }
        public SearchType SearchType { get; set; } = SearchType.Any;
    }
}