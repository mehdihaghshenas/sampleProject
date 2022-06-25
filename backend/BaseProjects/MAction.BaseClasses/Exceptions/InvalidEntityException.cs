using System;
using System.Collections.Generic;
using System.Net;

namespace MAction.BaseClasses.Exceptions
{
    public class InvalidEntityException : CustomApplicationException
    {
        public InvalidEntityException(Exception ex)
            : base(ex)
        {
        }

        public InvalidEntityException(string message) :
            base(message: message)
        {
        }

        public InvalidEntityException(Exception exception = null, Exception innerException = null, string message = null, string serviceName = null,
            string userName = null, int? userId = null, int? code = null, string tenant = null, Dictionary<string, string> errors = null, string projectName = null) :
            base(exception, innerException, message, serviceName, userName, userId, code, tenant, errors, projectName)
        {
        }

        public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
    }
}