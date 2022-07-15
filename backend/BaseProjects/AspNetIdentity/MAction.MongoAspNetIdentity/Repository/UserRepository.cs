using MongoDB.Bson;
using MongoDB.Driver;
using MAction.BaseMongoRepository;
using MAction.BaseClasses;
using MAction.AspNetIdentity.Base.Entities;
using Microsoft.AspNetCore.Identity;

namespace MAction.AspNetIdentity.Mongo.Repository;
public class UserRepository<TUser, TRole, TKey> : MongoRepository<TUser, TKey>, IUserRepository<TUser, TRole, TKey>
    where TUser : IdentityUser<TKey>, IUser, IBaseEntity, new()
    where TRole : IdentityRole<TKey>, IRole, IBaseEntity, new()
    where TKey : IEquatable<TKey>
{
    private readonly IRoleRepository<TRole, TKey> roleRepository;
    private readonly UserManager<TUser> _userManager;
    protected override string CollectionName => "Users";

    public UserRepository(IRoleRepository<TRole, TKey> roleRepository, IMongoDependencyProvider databaseName, IMongoClient mongoClient, IBaseServiceDependencyProvider baseServiceDependencyProvider, UserManager<TUser> userManager) : base(databaseName, mongoClient, baseServiceDependencyProvider)
    {
        this.roleRepository = roleRepository;
        _userManager = userManager;
    }

    public async Task<TUser> GetByEmailOrPhoneNumber(string input, CancellationToken cancellationToken)
    {
        var users = GetAll().Where(p => p.Email == input || p.PhoneNumber == input || p.UserName == input);
        return await Task.FromResult(users.FirstOrDefault());
    }

    public async Task<TUser> GetByPhoneNumber(string phone, CancellationToken cancellationToken)
    {
        return await Task.FromResult(GetAll().Where(p => p.PhoneNumber == phone).FirstOrDefault());
    }

    public async Task<bool> Exists(TKey id, string email, string phoneNumber, CancellationToken cancellationToken)
    {
        return await Task.FromResult(GetAll().Any(p => p.Id.ToString() != id.ToString() && (p.Email == email || p.PhoneNumber == phoneNumber)));
    }

    public async Task<List<TRole>> GetUserRoles(TKey id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        var userRoles = await _userManager.GetRolesAsync(user);
        return await Task.FromResult(roleRepository.GetAll().Where(p => userRoles.Contains(p.Name)).ToList());
    }
}