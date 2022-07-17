using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseClasses.Helpers;
using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using MAction.BaseServices.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MAction.BaseServices;

public class BaseServiceWithKey<TKey, TEntity, TInputModel, TOutputModel> : IBaseService<TKey, TEntity, TInputModel, TOutputModel>
    where TEntity : BaseEntity where TKey : new()
{
    protected readonly IMapper Mapper;
    private readonly IBaseServiceDependencyProvider _baseServiceDependencyProvider;
    private readonly IBaseRepository<TEntity, TKey> _repository;
    public void SetHasSystemPrivilege(bool value)
    {
        _baseServiceDependencyProvider.SetInternalMode(value);
        _repository.SetHasSystemPrivilege(value);
    }

    public BaseServiceWithKey(IBaseRepository<TEntity, TKey> repository, IMapper mapper, IBaseServiceDependencyProvider baseServiceDependencyProvider)
    {
        Mapper = mapper;
        _baseServiceDependencyProvider = baseServiceDependencyProvider;
        _repository = repository;
    }

    #region GetSet TEntity
    public DynamicQueryFilterResult<TOutputModel> GetItemByFilter(FilterAndSortConditions conditions,
        Expression<Func<TEntity, bool>> extraWhereCondition = null,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        return GetItemsByFilter<TOutputModel>(conditions, extraWhereCondition, mappingType);
    }

    public Task<DynamicQueryFilterResult<TOutputModel>> GetItemByFilterAsync(FilterAndSortConditions conditions,
        Expression<Func<TEntity, bool>> extraWhereCondition = null,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto,
        CancellationToken cancellationToken = default)
    {
        return GetItemsByFilterAsync<TOutputModel>(conditions, extraWhereCondition, mappingType, cancellationToken);
    }

    public DynamicQueryFilterResult<TEntity> GetItemsByFilter(FilterAndSortConditions filter,
        Expression<Func<TEntity, bool>> extraWhereCondition = null)
    {
        var query = GetFilterQuery(filter, extraWhereCondition, out var withoutPaging);
        return MapToDynamicFilterResult(filter, query, withoutPaging);
    }

    public Task<DynamicQueryFilterResult<TEntity>> GetItemsByFilterAsync(FilterAndSortConditions filter,
        Expression<Func<TEntity, bool>> extraWhereCondition = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetItemsByFilter(filter, extraWhereCondition));
    }
    protected IQueryable<TEntity> GetFilterQuery(FilterAndSortConditions filter,
    Expression<Func<TEntity, bool>> extraWhereCondition, out IQueryable<TEntity> withoutPaging)
    {
        var beforeSort = GetAll();

        if (filter != null && !string.IsNullOrEmpty(filter.WhereConditionText))
        {
            beforeSort = beforeSort.Where(filter.WhereConditionText);
        }

        beforeSort = extraWhereCondition != null ? beforeSort.Where(extraWhereCondition) : beforeSort.AsQueryable();
        var sortedQuery = beforeSort;

        if (filter != null && !string.IsNullOrEmpty(filter.SortText))
            sortedQuery = sortedQuery.OrderBy(filter.SortText);

        withoutPaging = beforeSort;
        if (filter == null || filter.DisablePaging == true)
            return sortedQuery;
        return sortedQuery.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
    }

    protected IQueryable<TEntity> GetAll()
    {
        var result = _repository.GetAll();
        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            result = result.Where(GetSelectPermissionExpression());

        return result;
    }

    public TEntity Insert(TEntity entity)
    {
        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            CheckInsertPermission(entity);

        return _repository.InsertWithSaveChange(entity);
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            CheckInsertPermission(entity);

        return await _repository.InsertWithSaveChangeAsync(entity);
    }

    public void Update(TEntity entity)
    {
        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            CheckUpdatePermission(entity);

        _repository.UpdateWithSaveChange(entity);
    }

    public async Task<int> UpdateAsync(TEntity entity)
    {
        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            CheckUpdatePermission(entity);

        return await _repository.UpdateWithSaveChangeAsync(entity);
    }

    #endregion

    #region GetWith TResult
    public TOutputModel Get(TKey entityId, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        var filter = ExpressionHelpers.GetIdFilter<TEntity>(entityId);
        var res = GetItemByFilter(new FilterAndSortConditions() { DisablePaging = null, PageNumber = 1, PageSize = 10 },
            filter, mappingType);
        if (res.Data.Count == 0)
            throw new InvalidEntityException();
        return res.Data.First();
    }

    public async Task<TOutputModel> GetAsync(TKey entityId,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto,
        CancellationToken cancellationToken = default)
    {
        var filter = ExpressionHelpers.GetIdFilter<TEntity>(entityId);
        var res = await GetItemByFilterAsync(
            new FilterAndSortConditions() { DisablePaging = null, PageNumber = 1, PageSize = 10 }, filter, mappingType,
            cancellationToken);
        if (res.Data.Count == 0)
            throw new InvalidEntityException();
        else
            return res.Data.First();
    }

    public DynamicQueryFilterResult<TResult> GetItemsByFilter<TResult>(FilterAndSortConditions conditions,
        Expression<Func<TEntity, bool>> extraWhereCondition = null,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        switch (mappingType)
        {
            case OutputModelMappingTypeEnum.UseMappingModel
                when this is not ISelectWithModelMapper<TEntity, TResult>:
                throw new NotImplementedException(
                    "please inherit your service from ISelectWithModelMapper<TEntity, TResult>");

            case OutputModelMappingTypeEnum.UseExpression
                when this is not ISelectWithModelExpression<TEntity, TResult>:
                throw new NotImplementedException(
                    "please inherit your service from ISelectWithModelExpression<TEntity, TResult>");

            case OutputModelMappingTypeEnum.Auto or OutputModelMappingTypeEnum.UseExpression
                when this is ISelectWithModelExpression<TEntity, TResult>:
                {
                    var selectExpr = (this as ISelectWithModelExpression<TEntity, TResult>)?.SelectExpression();
                    var resQuery = GetFilterQuery(conditions, extraWhereCondition, out var withOutPagingQuery)
                        .Select(selectExpr ?? throw new InvalidOperationException());
                    var res = MapToDynamicFilterResult(conditions, resQuery, withOutPagingQuery.Select(selectExpr));
                    return res;
                }
            case OutputModelMappingTypeEnum.Auto or OutputModelMappingTypeEnum.UseMappingModel
                when this is ISelectWithModelMapper<TEntity, TResult>:
                {
                    var res = MapToDynamicFilterResult(conditions,
                        GetFilterQuery(conditions, extraWhereCondition, out var withOutPagingQuery), withOutPagingQuery);

                    var t = new DynamicQueryFilterResult<TResult>()
                    {
                        Data = res.Data.Select(x => ((ISelectWithModelMapper<TEntity, TResult>)this).MapEntityToOutput(x))
                            .ToList(),
                        PageNumber = res.PageNumber,
                        PageSize = res.PageSize,
                        TotalCount = res.TotalCount
                    };
                    if (conditions?.DisablePaging != true) return t;
                    t.TotalCount = t.Data.Count;
                    t.PageSize = t.Data.Count;
                    return t;
                }
            case OutputModelMappingTypeEnum.Auto:
            case OutputModelMappingTypeEnum.UseAutoMapper:
                {
                    var q = GetFilterQuery(conditions, extraWhereCondition, out var withOutPagingQuery);
                    var count = conditions?.DisablePaging == null
                        ? conditions?.PageSize ?? 0 + 1
                        : withOutPagingQuery.Count();
                    var t = new DynamicQueryFilterResult<TResult>()
                    {
                        Data = Mapper.ProjectTo<TResult>(q).ToList(),
                        PageNumber = conditions?.PageNumber ?? 1,
                        PageSize = conditions?.PageSize ?? count,
                        TotalCount = count
                    };
                    if (conditions?.DisablePaging != true) return t;
                    t.TotalCount = t.Data.Count;
                    t.PageSize = t.Data.Count == 0 ? 10 : t.Data.Count;
                    return t;
                }
            default:
                {
                    var countTemp = 0;
                    var res = GetFilterQuery(conditions, extraWhereCondition, out var withOutPagingQuery);
                    if (conditions?.DisablePaging != null)
                        countTemp = withOutPagingQuery.Count();

                    var count = conditions?.DisablePaging == null ? conditions?.PageSize ?? 0 + 1 : countTemp;
                    var t = new DynamicQueryFilterResult<TResult>
                    {
                        Data = res.Select(x => Mapper.Map<TResult>(x)).ToList(),
                        PageNumber = conditions?.PageNumber ?? 1,
                        PageSize = conditions?.PageSize ?? count,
                        TotalCount = count
                    };

                    if (conditions?.DisablePaging != true) return t;
                    t.TotalCount = t.Data.Count;
                    t.PageSize = t.Data.Count == 0 ? 10 : t.Data.Count;
                    t.PageNumber = 1;
                    return t;
                }
        }
    }

    public Task<DynamicQueryFilterResult<TResult>> GetItemsByFilterAsync<TResult>(FilterAndSortConditions conditions,
        Expression<Func<TEntity, bool>> extraWhereCondition = null,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetItemsByFilter<TResult>(conditions, extraWhereCondition, mappingType));
    }

    private static DynamicQueryFilterResult<TResult> MapToDynamicFilterResult<TResult>(PageParams pageParams,
        IQueryable<TResult> query, IQueryable<TResult> withoutPagingQuery)
    {
        pageParams ??= new PageParams() { DisablePaging = true };
        query ??= pageParams.DisablePaging ?? false
            ? withoutPagingQuery
            : withoutPagingQuery.Skip((pageParams.PageNumber - 1) * pageParams.PageSize).Take(pageParams.PageSize);
        var resList = query.ToList();
        var res = new DynamicQueryFilterResult<TResult>
        {
            Data = resList,
            PageNumber = pageParams.PageNumber,
            PageSize = pageParams.DisablePaging == true ? resList.Count == 0 ? 10 : resList.Count : pageParams.PageSize,
            TotalCount = pageParams.DisablePaging switch
            {
                null => pageParams.PageSize + 1,
                true => resList.Count,
                _ => withoutPagingQuery.Count()
            }
        };
        return res;
    }
    #endregion

    #region Mapping Input and Output to entity
    private TEntity GetEntityFromInputModel(TInputModel inputModel, OutputModelMappingTypeEnum mappingType)
    {
        switch (mappingType)
        {
            case OutputModelMappingTypeEnum.UseExpression:
                throw new InvalidOperationException("Can not use Expression mode in input");
            case OutputModelMappingTypeEnum.UseMappingModel
                when this is not IInputModelCustomMapper<TEntity, TInputModel>:
                throw new InvalidOperationException("This class dos not contain CustomMapping implemented");
            case OutputModelMappingTypeEnum.Auto:
            case OutputModelMappingTypeEnum.UseAutoMapper:
            case OutputModelMappingTypeEnum.PureAutoMapper:
                break;
        }

        TEntity entity;
        if (this is IInputModelCustomMapper<TEntity, TInputModel> &&
            mappingType != OutputModelMappingTypeEnum.UseAutoMapper)
            entity = (this as IInputModelCustomMapper<TEntity, TInputModel>)?.MapInputToEnity(inputModel);
        else
            entity = Mapper.Map<TEntity>(inputModel);
        return entity;
    }

    private TOutputModel GetOutputModelFromEntity(TEntity entity, OutputModelMappingTypeEnum mappingType)
    {
        switch (mappingType)
        {
            case OutputModelMappingTypeEnum.UseExpression:
                throw new InvalidOperationException("Can not use Expression mode in input");
            case OutputModelMappingTypeEnum.UseMappingModel
                when !(this is ISelectWithModelMapper<TEntity, TOutputModel>):
                throw new InvalidOperationException("This class dos not contain CustomMapping implemented");
            case OutputModelMappingTypeEnum.Auto:
            case OutputModelMappingTypeEnum.UseAutoMapper:
            case OutputModelMappingTypeEnum.PureAutoMapper:
                break;
        }

        TOutputModel output;
        if (this is ISelectWithModelMapper<TEntity, TOutputModel> &&
            mappingType != OutputModelMappingTypeEnum.UseAutoMapper)
            output = ((ISelectWithModelMapper<TEntity, TOutputModel>)this).MapEntityToOutput(entity);
        else
            output = Mapper.Map<TOutputModel>(entity);
        return output;
    }
    #endregion

    #region Insert/Update with TInputModel

    public TOutputModel Insert(TInputModel inputModel,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        var entity = GetEntityFromInputModel(inputModel, mappingType);

        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            CheckInsertPermission(entity);

        entity = _repository.InsertWithSaveChange(entity);
        return GetOutputModelFromEntity(entity, mappingType);
    }

    public async Task<TOutputModel> InsertAsync(TInputModel inputModel,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto,
        CancellationToken cancellationToken = default)
    {
        var entity = GetEntityFromInputModel(inputModel, mappingType);
        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            CheckInsertPermission(entity);

        entity = await _repository.InsertWithSaveChangeAsync(entity, cancellationToken);
        return GetOutputModelFromEntity(entity, mappingType);
    }



    public void Update(TInputModel inputModel, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        var entity = GetEntityFromInputModel(inputModel, mappingType);
        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            CheckUpdatePermission(entity);

        _repository.UpdateWithSaveChange(entity);
    }

    public Task<int> UpdateAsync(TInputModel inputModel,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto,
        CancellationToken cancellationToken = default)
    {
        TEntity entity = GetEntityFromInputModel(inputModel, mappingType);
        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            CheckUpdatePermission(entity);

        return _repository.UpdateWithSaveChangeAsync(entity, cancellationToken);
    }
    #endregion

    public Task RemoveAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = _repository.Get(id);

        if (!_baseServiceDependencyProvider.HasSystemPrivilege())
            CheckDeletePermission(entity);

        return _repository.RemoveWithSaveChangeAsync(id, cancellationToken);
    }

    #region Perimission Management

    public virtual Expression<Func<TEntity, bool>> GetSelectPermissionExpression()
    {
        throw new NotImplementedException();
    }

    public virtual void CheckInsertPermission(TEntity entity)
    {
        throw new ForbiddenExpection();
    }

    public virtual void CheckUpdatePermission(TEntity entity)
    {
        throw new ForbiddenExpection();

    }

    public virtual void CheckDeletePermission(TEntity entity)
    {
        throw new ForbiddenExpection();

    }
    #endregion

    #region History Management
    public async Task<DynamicQueryFilterResult<EventHistoryOutputModel<TOutputModel, TKey>>> GetEventHistoryAsync(TKey entityId)
    {
        var outpout = await GetAsync(entityId);
        return new DynamicQueryFilterResult<EventHistoryOutputModel<TOutputModel, TKey>>()
        {
            Data = new List<EventHistoryOutputModel<TOutputModel, TKey>>()
            {
                new EventHistoryOutputModel<TOutputModel, TKey>()
                {
                    ChangesContent = outpout,
                    EventHistoryId = new TKey(),
                    EventHistoryTypeId = EventHistoryTypeEnum.Added,
                    RequestId = 1,
                    PrimeryKeyId = entityId
                }
            },
        };
    }
    #endregion
}