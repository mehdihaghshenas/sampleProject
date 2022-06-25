using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.AspNetIdentity.Base.ViewModel
{
    public class EmailAndVerificationCode_Request
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}
