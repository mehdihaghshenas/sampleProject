using MAction.AspNetIdentity.EFCore.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.AspNetIdentity.EFCore
{
    public class IdentityContext: IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
    }
}
