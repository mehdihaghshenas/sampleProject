using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseServices;

public class BaseServiceDependencyProvider : IBaseServiceDependencyProvider
{
    protected bool _internalMode;

    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected ITimeZoneConverterService _timeZoneConverterService;
    private readonly IAuthorizationService _authorizationService;

    public BaseServiceDependencyProvider(IHttpContextAccessor httpContextAccessor, ITimeZoneConverterService timeZoneConverterService,
        IAuthorizationService authorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _timeZoneConverterService = timeZoneConverterService;

        _internalMode = false;
        _authorizationService = authorizationService;
    }



    public object UserId
    {
        get => _internalMode ? SystemUserId :
            _httpContextAccessor.HttpContext.Items["UserId"] == null ? null : _httpContextAccessor.HttpContext.Items["UserId"];
    }
    public bool HasToken { get => _internalMode ? false : _httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"); }
    public bool IsAuthenticated => _internalMode ? false : _httpContextAccessor.HttpContext.User != null;


    public ITimeZoneConverterService TimeZoneConverterService
    {
        get => _timeZoneConverterService;
        set => _timeZoneConverterService = value;
    }

    public virtual void SetInternalMode(bool isInternalMode)
    {
        _internalMode = isInternalMode;
    }

    public virtual object SystemUserId { get => 1; }

    //[Authorize(Policy = PolicyTypes.Users.Manage)]
    public bool IsInRole(string RoleName)
    {
        return HasToken && _httpContextAccessor.HttpContext.User.IsInRole(RoleName);
    }
    public bool IsCurrentUserAuthorize(string policyName)
    {
        if (UserId == null)
            return false;

        var t =_authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, policyName);
        t.Wait();

        return t.Result.Succeeded;
    }

}
