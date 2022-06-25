using System;
using System.Collections.Generic;
using System.Net;

namespace MAction.BaseClasses.Exceptions;

public class BadRequestException : CustomApplicationException
{
    public BadRequestException(Exception ex)
        : base(ex)
    {
    }

    public BadRequestException(string message) :
        base(message: message)
    {
    }

    public BadRequestException(Exception exception = null, Exception innerException = null, string message = null, string serviceName = null,
        string userName = null, int? userId = null, int? code = null, string tenant = null, Dictionary<string, string> errors = null, string projectName = null) :
        base(exception, innerException, message, serviceName, userName, userId, code, tenant, errors, projectName)
    {
    }

    public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}