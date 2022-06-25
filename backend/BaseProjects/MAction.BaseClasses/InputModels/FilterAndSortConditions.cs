using System;

namespace MAction.BaseClasses.InputModels;

public class FilterAndSortConditions : PageAndSortParams
{
    public FilterAndSortConditions() : base()
    {
    }
    public FilterAndSortConditions(PageParams pageParams)
    {
        DisablePaging = pageParams.DisablePaging;
        PageNumber = pageParams.PageNumber;
        PageSize = pageParams.PageSize;
    }
    public FilterAndSortConditions(PageAndSortParams pageAndSort) : this((PageParams)pageAndSort)
    {
        SortText = pageAndSort.SortText;
    }
    public string WhereConditionText { get; set; }


}
