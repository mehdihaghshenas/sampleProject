using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace MAction.BaseClasses.Exceptions
{
    public abstract class CustomApplicationException : Exception
    {
        private string _CustomMessage { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public virtual string ServiceName { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public virtual string ProjectName { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public virtual string UserName { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public virtual int? UserId { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public virtual int? Code { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public virtual string Tenant { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public virtual Dictionary<string, string> Errors { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public abstract HttpStatusCode HttpStatusCode { get; }

        private Exception Exception { get; set; }
        public override string Message => _CustomMessage ?? Exception?.Message;

        public CustomApplicationException(Exception exception = null, Exception innerException = null, string message = null, string serviceName = null, string userName = null, int? userId = null, int? code = null, string tenant = null, Dictionary<string, string> errors = null, string projectName = null)
            : base(message ?? exception?.Message, innerException ?? exception?.InnerException)
        {
            _CustomMessage = message;
            Exception = exception;
            ServiceName = serviceName;
            UserName = userName;
            UserId = userId;
            Code = code;
            Tenant = tenant;
            Errors = errors;
            ProjectName = projectName;
        }
    }
}