

using MAction.BaseEFRepository;
using MAction.AspNetIdentity.EFCore.Domain;

namespace MAction.AspNetIdentity.EFCore.Repository;

public interface IUserRepository : IEFRepository<ApplicationUser> 
{
    Task<bool> Exists(int id, string email, string phoneNumber, CancellationToken cancellationToken);
    Task<ApplicationUser> GetByEmailOrPhoneNumber(string input, CancellationToken cancellationToken);
    Task<ApplicationUser?> GetByPhoneNumber(string phone, CancellationToken cancellationToken);
    Task<List<ApplicationRole>> GetUserRoles(int id, CancellationToken cancellationToken);
}
