using System.Collections.Generic;

namespace MAction.BaseClasses.InputModels;

public class EntityTitleListFilterAndSelector
{
    public string KeyColumn { get; set; }
    public List<string> ShowColumn { get; set; }
    public FilterAndSortConditions Filter { get; set; } = new FilterAndSortConditions();
    public bool DistinctResult { get; set; }
    public string Seprator { get; set; } = " _ ";
}
