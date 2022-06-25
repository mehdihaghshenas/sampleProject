using MAction.BaseMongoRepository;
using MongoDB.Bson;
using MAction.AspNetIdentity.Mongo.Domain;

namespace MAction.AspNetIdentity.Mongo.Repository;

public interface IUserRepository : IMongoRepository<ApplicationUser>
{
    Task<bool> Exists(ObjectId id, string email, string phoneNumber, CancellationToken cancellationToken);
    Task<ApplicationUser> GetByEmailOrPhoneNumber(string input, CancellationToken cancellationToken);
    Task<ApplicationUser> GetByPhoneNumber(string phone, CancellationToken cancellationToken);
    Task<List<ApplicationRole>> GetUserRoles(ObjectId id, CancellationToken cancellationToken);
}
