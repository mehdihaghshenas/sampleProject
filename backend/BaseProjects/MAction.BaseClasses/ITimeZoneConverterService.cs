using System;
using System.Collections.Generic;
using MAction.BaseClasses.OutpuModels;

namespace MAction.BaseClasses;

public interface ITimeZoneConverterService
{
    string IanaToWindows(string IanaCode);

    string WindowsIana(string WindowsCode);
    List<TimezoneConvertOutputViewModel> ConvertIanaTimezonestoUtc(List<string> IanaCodes);
    /// <summary>
    /// Returns the client timezone info by parsing the header. If location was specified, its TimeZoneInfo will be computed based on the provided user's location.
    /// Otherwise, the timeZone sent in header will be converted to the TimeZoneInfo
    /// </summary>
    /// <returns></returns>
    TimeZoneInfo GetClientTimeZoneInfo();
    Tuple<double, double> GetClientLocation();

}
