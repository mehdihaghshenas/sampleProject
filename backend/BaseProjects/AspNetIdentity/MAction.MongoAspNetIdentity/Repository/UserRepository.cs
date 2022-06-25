using MAction.BaseMongoRepository;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MAction.AspNetIdentity.Mongo.Domain;

namespace MAction.AspNetIdentity.Mongo.Repository;
public class UserRepository : MongoRepository<ApplicationUser>, IUserRepository
{
    private readonly IRoleRepository roleRepository;
    protected override string CollectionName => "Users";

    public UserRepository(IRoleRepository roleRepository, IMongoDependencyProvider databaseName, IMongoClient mongoClient) : base(databaseName, mongoClient)
    {
        this.roleRepository = roleRepository;
    }

    public async Task<ApplicationUser> GetByEmailOrPhoneNumber(string input, CancellationToken cancellationToken)
    {
        var users = GetAll().Where(p => p.Email == input || p.PhoneNumber == input || p.UserName == input);
        return await Task.FromResult(users.FirstOrDefault());
    }

    public async Task<ApplicationUser> GetByPhoneNumber(string phone, CancellationToken cancellationToken)
    {
        return await Task.FromResult(GetAll().Where(p => p.PhoneNumber == phone).FirstOrDefault());
    }

    public async Task<bool> Exists(ObjectId id, string email, string phoneNumber, CancellationToken cancellationToken)
    {
        return await Task.FromResult(GetAll().Any(p => p.Id != id && (p.Email == email || p.PhoneNumber == phoneNumber)));
    }

    public async Task<List<ApplicationRole>> GetUserRoles(ObjectId id, CancellationToken cancellationToken)
    {
        var userRoles = (await GetAsync(id)).Roles.Select(x => ObjectId.Parse(x));
        return await Task.FromResult(roleRepository.GetAll().Where(p => userRoles.Contains(p.Id)).ToList());
    }

}
