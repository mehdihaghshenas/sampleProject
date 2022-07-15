using System.Security.Claims;
using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;

namespace MAction.AspNetIdentity.Base;

public interface IRoleService
{
    Task<IEnumerable<PolicyDto>> GetAllPolicies(CancellationToken cancellationToken);

    Task<IEnumerable<Claim>> GetRolePolicy(string role, CancellationToken cancellationToken);
    Task SetPolicies(IEnumerable<string> addPolicies, IEnumerable<string> removePolicies,
        string role, CancellationToken cancellationToken);
    Task CreateRole(string role, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetRoles();
}