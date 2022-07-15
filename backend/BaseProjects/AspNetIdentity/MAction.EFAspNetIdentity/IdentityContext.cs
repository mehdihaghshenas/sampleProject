using MAction.AspNetIdentity.Base.Entities;
using MAction.BaseClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.AspNetIdentity.EFCore;

public class IdentityContext<TUser, TRole, TKey>: IdentityDbContext<TUser, TRole, TKey>
    where TUser : IdentityUser<TKey>, IUser, IBaseEntity, new()
    where TRole : IdentityRole<TKey>, IRole, IBaseEntity, new()
    where TKey : IEquatable<TKey>
{
}
