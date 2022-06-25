using MAction.AspNetIdentity.EFCore.Domain;
using MAction.BaseEFRepository;

namespace MAction.AspNetIdentity.EFCore.Repository
{
    public class RoleRepository : EFRepository<ApplicationRole>, IRoleRepository
    {
        public RoleRepository(IdentityContext context) : base(context)
        {
        }

    }

    public interface IRoleRepository : IEFRepository<ApplicationRole>
    {
    }
}
