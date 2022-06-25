using MAction.BaseEFRepository;
using MAction.AspNetIdentity.EFCore.Domain;
using Microsoft.EntityFrameworkCore;

namespace MAction.AspNetIdentity.EFCore.Repository;
public class UserRepository : EFRepository<ApplicationUser>, IUserRepository
{
    private readonly IRoleRepository roleRepository;
    private readonly IdentityContext _identitycontext;
    public UserRepository(IdentityContext context, IRoleRepository roleRepository) : base(context)
    {
        this.roleRepository = roleRepository;
        _identitycontext= context;
    }

    public async Task<ApplicationUser> GetByEmailOrPhoneNumber(string input, CancellationToken cancellationToken)
    {
        var users = GetAll().Where(p => p.Email == input || p.PhoneNumber == input || p.UserName == input);
        return await users.FirstAsync();
    }

    public async Task<ApplicationUser?> GetByPhoneNumber(string phone, CancellationToken cancellationToken)
    {
        return await GetAll().Where(p => p.PhoneNumber == phone).FirstOrDefaultAsync();
    }

    public async Task<bool> Exists(int id, string email, string phoneNumber, CancellationToken cancellationToken)
    {
        return await Task.FromResult(GetAll().Any(p => p.Id != id && (p.Email == email || p.PhoneNumber == phoneNumber)));
    }

    public async Task<List<ApplicationRole>> GetUserRoles(int id, CancellationToken cancellationToken)
    {
        var userRoles = await _identitycontext.UserRoles.Where(q => q.UserId == id).Select(p => p.RoleId).ToListAsync(cancellationToken);
        return await roleRepository.GetAll().Where(p => userRoles.Contains(p.Id)).ToListAsync();
    }

}
