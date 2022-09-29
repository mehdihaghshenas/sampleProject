using MAction.BaseEFRepository;
using Microsoft.EntityFrameworkCore;
using MAction.BaseClasses;
using MAction.AspNetIdentity.Base.Entities;
using Microsoft.AspNetCore.Identity;

namespace MAction.AspNetIdentity.EFCore.Repository;
#nullable disable
public class UserRepository<TContext, TUser, TRole, TKey> : EFRepository<TUser, TKey>, IUserRepository<TUser, TRole, TKey>
    where TUser : IdentityUser<TKey>, IUser, IBaseEntity, new()
    where TKey : IEquatable<TKey>
    where TRole : IdentityRole<TKey>, IRole, IBaseEntity, new()
    where TContext : IdentityContext<TUser, TRole, TKey>
{
    private readonly IRoleRepository<TRole, TKey> _roleRepository;
    private readonly IdentityContext<TUser, TRole, TKey> _identityContext;
    public UserRepository(TContext context, IRoleRepository<TRole, TKey> roleRepository, IBaseServiceDependencyProvider baseServiceDependencyProvider) : base(context, baseServiceDependencyProvider)
    {
        this._roleRepository = roleRepository;
        _identityContext = context;
    }

    public async Task<TUser> GetByEmailOrPhoneNumber(string input, CancellationToken cancellationToken)
    {
        var users = GetAll().Where(p => p.Email == input || p.PhoneNumber == input || p.UserName == input);
        return await users.FirstAsync();
    }

    public async Task<TUser> GetByPhoneNumber(string phone, CancellationToken cancellationToken)
    {
        return await GetAll().Where(p => p.PhoneNumber == phone).FirstOrDefaultAsync();
    }

    public async Task<bool> Exists(TKey id, string email, string phoneNumber, CancellationToken cancellationToken)
    {
        return await Task.FromResult(GetAll().Any(p => p.Id.ToString() != id.ToString() && (p.Email == email || p.PhoneNumber == phoneNumber)));
    }

    public async Task<List<TRole>> GetUserRoles(TKey id, CancellationToken cancellationToken)
    {
        var userRoles = await _identityContext.UserRoles.Where(q => q.UserId.ToString() == id.ToString()).Select(p => p.RoleId).ToListAsync(cancellationToken);
        return await _roleRepository.GetAll().Where(p => userRoles.Contains(p.Id)).ToListAsync();
    }

}
#nullable enable