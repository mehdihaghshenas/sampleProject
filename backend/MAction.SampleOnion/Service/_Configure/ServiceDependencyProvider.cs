using MAction.BaseClasses;
using MAction.BaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.SipOnline.Service
{
    public interface IServiceDependencyProvider : IBaseServiceDependencyProvider
    {
        public new ObjectId UserId { get; }
    }
    public class ServiceDependencyProvider : BaseServiceDependencyProvider, IServiceDependencyProvider
    {
        public ServiceDependencyProvider(IHttpContextAccessor httpContextAccessor, ITimeZoneConverterService timeZoneConverterService, IAuthorizationService authorizationService) : base(httpContextAccessor, timeZoneConverterService, authorizationService)
        {
        }
        public new ObjectId UserId
        {
            get
            {
                return ObjectId.Parse(base.UserId.ToString());
            }
        }

    }
}
