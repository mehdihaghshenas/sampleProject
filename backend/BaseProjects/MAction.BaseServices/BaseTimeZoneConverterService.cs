using System;
using System.Collections.Generic;
using System.Linq;
using MAction.BaseServices.ViewModel;
using Microsoft.AspNetCore.Http;
using TimeZoneConverter;

namespace MAction.BaseServices;

public class BaseTimeZoneConverterService : ITimeZoneConverterService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BaseTimeZoneConverterService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string IanaToWindows(string IanaCode)
    {
        if (!string.IsNullOrEmpty(IanaCode))
            return TZConvert.IanaToWindows(IanaCode);
        return string.Empty;
    }

    public string WindowsIana(string WindowsCode)
    {
        if (!string.IsNullOrEmpty(WindowsCode))
            return TZConvert.WindowsToIana(WindowsCode);
        return string.Empty;

    }
    public List<TimezoneConvertOutputViewModel> ConvertIanaTimezonestoUtc(List<string> IanaCodes)
    {
        return IanaCodes.Select(t =>
             new TimezoneConvertOutputViewModel
             {
                 IanaCode = t,
                 StandardTimeZone = IanaToWindows(t)
             }
        ).ToList();

    }
    public Tuple<double, double> GetClientLocation()
    {
        try
        {
            var locationInHeader = _httpContextAccessor.HttpContext?.Request.Headers["location"].ToString().Split(',');
            if (locationInHeader == null || locationInHeader.Length == 1 || locationInHeader.Sum(c => double.Parse(c)) == 0)
            {
                return null;
            }
            var lng = double.Parse(locationInHeader[0]);
            var lat = double.Parse(locationInHeader[1]);

            var location = new Tuple<double, double>(lng, lat);
            return location;
        }
        catch (Exception)
        {
            return null;
        }
    }
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public virtual TimeZoneInfo GetClientTimeZoneInfo()
    {
        string timeZoneInHeader = null;
        if (_httpContextAccessor.HttpContext != null)
            timeZoneInHeader = _httpContextAccessor.HttpContext.Request.Headers["timezone"].ToString();

        var timeZoneName = timeZoneInHeader;

        if (string.IsNullOrWhiteSpace(timeZoneName))
            timeZoneName = TZConvert.GetTimeZoneInfo(TimeZoneInfo.Utc.Id).Id;

        return TZConvert.GetTimeZoneInfo(timeZoneName);
    }

}
