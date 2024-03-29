﻿using AutoMapper;
using MAction.BaseClasses;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.SipOnline.Service;

public interface IBaseServiceObjectId<TEntity, TInputModel, TOutputModel> : BaseServices.IBaseService<ObjectId, TEntity, TInputModel, TOutputModel>
where TEntity : BaseEntity
{
}
public class BaseServiceObjectId<TEntity, TInputModel, TOutputModel> : BaseServices.BaseServiceWithKey<ObjectId, TEntity, TInputModel, TOutputModel>, IBaseServiceObjectId<TEntity, TInputModel, TOutputModel>
    where TEntity : BaseEntity
{
    public BaseServiceObjectId(IBaseRepository<TEntity, ObjectId> repository, IMapper mapper, IBaseServiceDependencyProvider baseServiceDependencyProvider) : base(repository, mapper, baseServiceDependencyProvider)
    {
    }
}

