using System.Security.Claims;
using MAction.AspNetIdentity.Base;
using MAction.AspNetIdentity.Base.Entities;
using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using Microsoft.AspNetCore.Identity;

namespace MAction.AspNetIdentity.Base.Services;

public class RoleService<TRole, TKey> : IRoleService
    where TRole : IdentityRole<TKey>, IRole, IBaseEntity, new()
    where TKey : IEquatable<TKey>
{
    private readonly RoleManager<TRole> _roleManager;
    private readonly IBaseServiceDependencyProvider _dependencyProvider;

    public RoleService(RoleManager<TRole> roleManager, IBaseServiceDependencyProvider dependencyProvider)
    {
        _roleManager = roleManager;
        _dependencyProvider = dependencyProvider;
    }

    public Task<IEnumerable<PolicyDto>> GetAllPolicies(CancellationToken cancellationToken)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize(SecurityPolicies.AllowToViewPermission))
            throw new ForbiddenExpection($"You need {SecurityPolicies.AllowToViewPermission}");

        var result = PolicyLoader.GetAllPolicies();

        return Task.FromResult<IEnumerable<PolicyDto>>(result);
    }

    public async Task<IEnumerable<Claim>> GetRolePolicy(string role, CancellationToken cancellationToken)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize(SecurityPolicies.AllowToViewPermission))
            throw new ForbiddenExpection($"You need {SecurityPolicies.AllowToViewPermission}");

        var appRole = await _roleManager.FindByNameAsync(role);
        var result = await _roleManager.GetClaimsAsync(appRole);

        return result;
    }

    public async Task SetPolicies(IEnumerable<string> addPolicies, IEnumerable<string> removePolicies,
        string role, CancellationToken cancellationToken)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize(SecurityPolicies.AllowToManagePermission))
            throw new ForbiddenExpection($"You need {SecurityPolicies.AllowToManagePermission}");

        var appRole = await _roleManager.FindByNameAsync(role);
        var currentClaims = await _roleManager.GetClaimsAsync(appRole);

        foreach (var claim in addPolicies.Select(add => new Claim(PolicyLoader.CustomClaimTypes.ClaimType, add))
                     .Where(claim => currentClaims.FirstOrDefault(claim1 => claim1.Value == claim.Value) == null))
        {
            await _roleManager.AddClaimAsync(appRole, claim);
        }

        foreach (var claim in removePolicies
                     .Select(delete => new Claim(PolicyLoader.CustomClaimTypes.ClaimType, delete)).Where(claim =>
                         currentClaims.FirstOrDefault(claim2 => claim2.Value == claim.Value) != null))
        {
            await _roleManager.RemoveClaimAsync(appRole, claim);
        }
    }

    public async Task CreateRole(string role, CancellationToken cancellationToken)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize(SecurityPolicies.AllowToManageRoles))
            throw new ForbiddenExpection($"You need {SecurityPolicies.AllowToManageRoles}");

        await _roleManager.CreateAsync(new TRole()
        { Name = role, NormalizedName = role.ToUpper() });
    }

    public Task<IEnumerable<string>> GetRoles()
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize(SecurityPolicies.AllowToViewRoles))
            throw new ForbiddenExpection($"You need {SecurityPolicies.AllowToViewRoles}");

        var result = _roleManager.Roles.Select(x => x.Name);

        return Task.FromResult<IEnumerable<string>>(result);
    }
}