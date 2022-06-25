using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MAction.BaseClasses.OutpuModels;

public class DynamicQueryFilterResult<T>
{
    public ICollection<T> Data { get; set; }
    /// <summary>
    /// Total Count of Data
    /// </summary>
    [Description("Total Count of Data")]
    public int TotalCount { get; set; }
    /// <summary>
    /// page number of request
    /// </summary>
    public int? PageNumber { get; set; }
    /// <summary>
    /// count of item in each page
    /// </summary>
    public int PageSize { get; set; }
}
