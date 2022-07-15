namespace MAction.BaseClasses;

public interface IBaseServiceDependencyProvider
{
    object UserId { get; }
    bool HasToken { get; }
    bool IsInRole(string RoleName);
    bool IsCurrentUserAuthorize(string policyName);
    /// <summary>
    /// User ID of SystemUser
    /// system user is the userid that core use to do jobs and has no privillages
    /// </summary>
    object SystemUserId { get; }
    ITimeZoneConverterService TimeZoneConverterService { get; set; }
    void SetInternalMode(bool isInternalMode);
    bool HasSystemPrivilege();

}

