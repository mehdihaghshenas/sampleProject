using MAction.AspNetIdentity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.SampleOnion.Service.Configure
{

    public class DashboardPolicies : PoliciesBase
    {
        public static string AllowToSeeTest { get; } = $"{nameof(DashboardPolicies)}.{nameof(AllowToSeeTest)}";

    }
}
