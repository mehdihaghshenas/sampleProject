using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.AspNetIdentity.Base.Entities
{
    public interface IUser 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; }
    }

    public interface IRole
    {

    }
}
