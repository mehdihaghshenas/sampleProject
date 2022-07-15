using System;
using System.ComponentModel;

namespace MAction.BaseClasses.InputModels;

public class PageParams
{
    bool _passPrivilege = false;
    public bool GetPrivliage()
    {
        return _passPrivilege;
    }
    public void SetPrivilege()
    {
        _passPrivilege = true;
    }
    public PageParams()
    {
        PageSize = 10;
        PageNumber = 1;
    }
    int _pageSize = 10;
    [DefaultValue(10)]
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            if (value <= 1)
                throw new Exception("Page size must grater than one.");
            _pageSize = value;

        }
    }

    int _pageNumber = 1;
    [DefaultValue(1)]
    public int PageNumber
    {
        get
        {
            return _pageNumber;
        }
        set
        {
            if (_pageNumber <= 0)
                throw new Exception("Page number must grater than zero.");
            _pageNumber = value;
        }
    }

    /// <summary>
    /// Null to No Total Count
    /// </summary>
    [DefaultValue(false)]
    public bool? DisablePaging { get; set; } = false;
}