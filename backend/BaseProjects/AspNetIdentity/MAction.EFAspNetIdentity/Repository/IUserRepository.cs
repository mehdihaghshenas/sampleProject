

using MAction.AspNetIdentity.Base.Entities;
using MAction.BaseClasses;
using MAction.BaseEFRepository;
using Microsoft.AspNetCore.Identity;

namespace MAction.AspNetIdentity.EFCore.Repository;

public interface IUserRepository<TUser, TRole, TKey> : IEFRepository<TUser, TKey>
    where TUser : IdentityUser<TKey>, IUser, IBaseEntity, new()
    where TKey : IEquatable<TKey>
    where TRole : IdentityRole<TKey>, IRole, new()
{
    Task<bool> Exists(TKey id, string email, string phoneNumber, CancellationToken cancellationToken);
    Task<TUser> GetByEmailOrPhoneNumber(string input, CancellationToken cancellationToken);
    Task<TUser?> GetByPhoneNumber(string phone, CancellationToken cancellationToken);
    Task<List<TRole>> GetUserRoles(TKey id, CancellationToken cancellationToken);
}
