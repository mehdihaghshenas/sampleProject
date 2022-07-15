using System;
using System.Collections.Generic;
using System.Net;

namespace MAction.BaseClasses.Exceptions
{
    public class CustomException : CustomApplicationException
    {
        public CustomException(Exception ex)
            : base(ex)
        {
        }

        public CustomException(string message) :
            base(message: message)
        {
        }

        public CustomException(Exception exception = null, Exception innerException = null, string message = null, string serviceName = null,
            string userName = null, int? userId = null, int? code = null, string tenant = null, Dictionary<string, List<string>> errors = null, string projectName = null) :
            base(exception, innerException, message, serviceName, userName, userId, code, tenant, errors, projectName)
        {
        }

        public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
    }
}