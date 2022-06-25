using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseClasses.Exceptions
{
    [Serializable]
    public class ForbiddenExpection : CustomApplicationException
    {

        public ForbiddenExpection(Exception ex)
            : base(ex)
        {
        }

        public ForbiddenExpection(string message) :
            base(message: message)
        {
        }

        public ForbiddenExpection(Exception exception = null, Exception innerException = null, string message = null, string serviceName = null,
            string userName = null, int? userId = null, int? code = null, string tenant = null, Dictionary<string, string> errors = null, string projectName = null) :
            base(exception, innerException, message, serviceName, userName, userId, code, tenant, errors, projectName)
        {
        }
        public override HttpStatusCode HttpStatusCode => HttpStatusCode.Forbidden;
    }
}
