using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.AspNetIdentity.Base.ViewModel
{
    public class SendLoginVerificationCode_Response
    {
        public SendLoginVerificationCode_Response(bool userHasPassword, bool sendVerificationCode)
        {
            UserHasPassword = userHasPassword;
            SendVerificationCode = sendVerificationCode;
        }

        public bool UserHasPassword { get; set; }
        public bool SendVerificationCode { get; set; }
    }
}
