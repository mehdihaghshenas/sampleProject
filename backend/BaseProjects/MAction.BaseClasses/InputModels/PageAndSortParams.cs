namespace MAction.BaseClasses.InputModels;

public class PageAndSortParams : PageParams
{
    public PageAndSortParams() : base()
    {
    }
    public string SortText { get; set; }
}
