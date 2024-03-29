﻿using System.Linq;
using AutoMapper;
using Newtonsoft.Json;

namespace MAction.BaseClasses;

public abstract class BaseDTO<TEntity, TDto> : IBaseDto where TEntity : IBaseEntity, new()
    where TDto : class, new()
{
    protected IMappingExpression<TDto, TEntity> DtoToDomainMapping { get; private set; }
    protected IMappingExpression<TEntity, TDto> DomainToDtoMapping { get; private set; }

    public virtual void CustomReverseMapping(IMappingExpression<TEntity, TDto> mapping)
    {
    }

    public virtual void CustomMapping(IMappingExpression<TDto, TEntity> mapping)
    {
    }

    public BaseDTO()
    {
        //var configuration =
        //    new MapperConfiguration(cfg =>
        //    {
        //        DomainToDtoMapping = cfg.CreateMap<TEntity, TDto>();
        //        DtoToDomainMapping = cfg.CreateMap<TDto, TEntity>();
        //    });

        //var refProperties = from p in typeof(TEntity).GetProperties()
        //    where p.PropertyType.BaseType == typeof(BaseEntity)
        //    select p;

        //foreach (var prop in refProperties)
        //{
        //    DtoToDomainMapping.ForMember(prop.Name, m => m.Ignore());
        //}
    }

    public TEntity ToEntity(IMapper _mapper)
    {
        return _mapper.Map<TEntity>(this);
    }

    public static TDto FromEntity(TEntity entity, IMapper _mapper)
    {
        return _mapper.Map<TDto>(entity);
    }

    public TDto FromJson(string json, IMapper _mapper)
    {
        var result = JsonConvert.DeserializeObject(json);

        return _mapper.Map<TDto>(result);
    }

    public void ConfigureMapping(Profile profile)
    {
        var t = profile.CreateMap<TDto, TEntity>();
        var q = profile.CreateMap<TEntity, TDto>();

        this.CustomMapping(t);
        this.CustomReverseMapping(q);
    }
}