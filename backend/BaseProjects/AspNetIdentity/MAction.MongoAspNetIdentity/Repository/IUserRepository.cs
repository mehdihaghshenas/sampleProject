using MongoDB.Bson;
using MAction.BaseMongoRepository;
using MAction.AspNetIdentity.Base.Entities;
using Microsoft.AspNetCore.Identity;
using MAction.BaseClasses;

namespace MAction.AspNetIdentity.Mongo.Repository;

public interface IUserRepository<TUser, TRole, TKey> : IMongoRepository<TUser, TKey>
    where TUser : IdentityUser<TKey>, IUser, IBaseEntity, new()
    where TKey : IEquatable<TKey>
    where TRole : IdentityRole<TKey>, IRole, new()
{
    Task<bool> Exists(TKey id, string email, string phoneNumber, CancellationToken cancellationToken);
    Task<TUser> GetByEmailOrPhoneNumber(string input, CancellationToken cancellationToken);
    Task<TUser> GetByPhoneNumber(string phone, CancellationToken cancellationToken);
    Task<List<TRole>> GetUserRoles(TKey id, CancellationToken cancellationToken);
}
