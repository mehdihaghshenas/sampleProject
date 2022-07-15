using MAction.BaseClasses;
using MAction.BaseClasses.OutpuModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace MAction.TestHelpers;

public class FakeBaseServiceDependencyProvider : IBaseServiceDependencyProvider
{
    public object UserId { get; set; }

    public bool HasToken { get; set; }

    public object SystemUserId => 1;

    bool internalMode = false;

    public ITimeZoneConverterService TimeZoneConverterService { get; set; } = new FakeTimeZoneConverterService();
    public List<string> PermissionService { get; private set; }
    public List<string> Roles { get; private set; }


    public FakeBaseServiceDependencyProvider(bool isAdmin = false, string policyName = "CanDoSomethings", bool hasToken = true)
    {
        PermissionService = new List<string>();
        Roles = new List<string>();
        UserId = 1;
        HasToken = hasToken;
        PermissionService.Add(policyName);
    }

    public FakeBaseServiceDependencyProvider RemovePolicy(string policyName)
    {
        PermissionService.Remove(policyName);
        return this;
    }
    
    public FakeBaseServiceDependencyProvider RemoveAllPolicy()
    {
        PermissionService.Clear();
        return this;
    }
    public FakeBaseServiceDependencyProvider WithPolicy(string policyName)
    {
        PermissionService.Add(policyName);
        return this;
    }
    public FakeBaseServiceDependencyProvider WithRole(string roleName)
    {
        Roles.Add(roleName.ToUpper());
        return this;
    }

    public FakeBaseServiceDependencyProvider WithUserId(object userId)
    {
        UserId = userId;
        return this;
    }

    public bool IsInRole(string RoleName)
    {
        return Roles.Contains(RoleName.ToUpper());
    }

    public bool IsCurrentUserAuthorize(string policyName)
    {
        if (PermissionService.Contains(policyName) || IsInRole("admin"))
            return true;
        else
            return false;
    }

    public void SetInternalMode(bool isInternalMode)
    {
        internalMode = isInternalMode;
    }

    public bool HasSystemPrivilege()
    {
        return internalMode;
    }
}
internal class FakeTimeZoneConverterService : ITimeZoneConverterService
{
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
        return new Tuple<double, double>(2.0, 11.00);
    }

    public TimeZoneInfo GetClientTimeZoneInfo()
    {
        return TimeZoneInfo.Local;
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
}