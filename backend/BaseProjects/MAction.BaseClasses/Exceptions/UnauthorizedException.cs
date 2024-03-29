﻿using System;
using System.Collections.Generic;
using System.Net;

namespace MAction.BaseClasses.Exceptions
{
    public class UnauthorizedException : CustomApplicationException
    {
        public UnauthorizedException(Exception ex)
            : base(ex)
        {
        }

        public UnauthorizedException(string message) :
            base(message: message)
        {
        }

        public UnauthorizedException(Exception exception = null, Exception innerException = null, string message = null, string serviceName = null,
            string userName = null, int? userId = null, int? code = null, string tenant = null, Dictionary<string, List<string>> errors = null, string projectName = null) :
            base(exception, innerException, message, serviceName, userName, userId, code, tenant, errors, projectName)
        {
        }

        public override HttpStatusCode HttpStatusCode => HttpStatusCode.Unauthorized;
    }
}