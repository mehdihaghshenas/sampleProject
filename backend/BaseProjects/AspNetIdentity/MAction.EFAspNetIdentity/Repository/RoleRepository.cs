using MAction.AspNetIdentity.Base.Entities;
using MAction.BaseClasses;
using MAction.BaseEFRepository;
using Microsoft.AspNetCore.Identity;

namespace MAction.AspNetIdentity.EFCore.Repository
{
    public class RoleRepository<TUser, TRole, TKey> : EFRepository<TRole, TKey>, IRoleRepository<TRole, TKey>
        where TUser : IdentityUser<TKey>, IUser, IBaseEntity, new()
        where TKey : IEquatable<TKey>
        where TRole : IdentityRole<TKey>, IRole, IBaseEntity, new()
    {
        public RoleRepository(IdentityContext<TUser, TRole, TKey> context, IBaseServiceDependencyProvider baseServiceDependencyProvider) : base(context, baseServiceDependencyProvider)
        {
        }

    }

    public interface IRoleRepository<TRole, TKey> : IEFRepository<TRole, TKey>
        where TRole : IdentityRole<TKey>, IRole, IBaseEntity, new()
        where TKey : IEquatable<TKey>
    {
    }
}
