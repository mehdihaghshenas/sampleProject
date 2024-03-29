﻿using System;
using System.Collections.Generic;
using System.Net;

namespace MAction.BaseClasses.Exceptions
{
    public class NotFoundException : CustomApplicationException
    {
        public NotFoundException(Exception ex)
            : base(ex)
        {
        }

        public NotFoundException(string message) :
            base(message: message)
        {
        }

        public NotFoundException(Exception exception = null, Exception innerException = null, string message = null, string serviceName = null,
            string userName = null, int? userId = null, int? code = null, string tenant = null, Dictionary<string, List<string>> errors = null, string projectName = null) :
            base(exception, innerException, message, serviceName, userName, userId, code, tenant, errors, projectName)
        {
        }

        public override HttpStatusCode HttpStatusCode => HttpStatusCode.NotFound;
    }
}