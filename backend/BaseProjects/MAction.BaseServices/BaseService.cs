using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseClasses.Helpers;
using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MAction.BaseServices;

public class BaseService<TEntity, InputModel, OutputModel> : IBaseService<TEntity, InputModel, OutputModel> where TEntity : BaseEntity
{
    private readonly IBaseRepository<TEntity> _repository;
    private readonly IMapper _mapper;

    public BaseService(IBaseRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public OutputModel Get(object entityId, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        var filter = ExpressionHelpers.GetIdFilter<TEntity>(entityId);
        var res = GetItemByFilter(new FilterAndSortConditions() { DisablePaging = null, PageNumber = 1, PageSize = 10 }, filter, mappingType);
        if (res.Data.Count == 0)
            throw new InvalidEntityException();
        else
            return res.Data.First();
    }

    public async Task<OutputModel> GetAsync(object entityId, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto, CancellationToken cancellationToken = default)
    {
        var filter = ExpressionHelpers.GetIdFilter<TEntity>(entityId);
        var res = await GetItemByFilterAsync(new FilterAndSortConditions() { DisablePaging = null, PageNumber = 1, PageSize = 10 }, filter, mappingType);
        return res.Data.First();
    }

    public DynamicQueryFilterResult<OutputModel> GetItemByFilter(FilterAndSortConditions conditions, Expression<Func<TEntity, bool>> extraWhereCondition = null, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        return GetItemsByFilter<OutputModel>(conditions, extraWhereCondition, mappingType);
    }

    public Task<DynamicQueryFilterResult<OutputModel>> GetItemByFilterAsync(FilterAndSortConditions conditions, Expression<Func<TEntity, bool>> extraWhereCondition = null, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto, CancellationToken cancellationToken = default)
    {
        return GetItemsByFilterAsync<OutputModel>(conditions, extraWhereCondition, mappingType);
    }

    public DynamicQueryFilterResult<TEntity> GetItemsByFilter(FilterAndSortConditions filter, Expression<Func<TEntity, bool>> extraWhereCondition = null)
    {
        var query = GetFilterQuery(filter, extraWhereCondition, out var withoutPaging);
        return MapToDynamicFilterResult(filter, query, withoutPaging);

    }

    public Task<DynamicQueryFilterResult<TEntity>> GetItemsByFilterAsync(FilterAndSortConditions filter, Expression<Func<TEntity, bool>> extraWhereCondition = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetItemsByFilter(filter, extraWhereCondition));
    }
    public DynamicQueryFilterResult<TResult> GetItemsByFilter<TResult>(FilterAndSortConditions conditions, Expression<Func<TEntity, bool>> extraWhereCondition = null, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        if ((mappingType == OutputModelMappingTypeEnum.Auto || mappingType == OutputModelMappingTypeEnum.UseExpression) && this is ISelectWithModelExpression<TEntity, TResult>)
        {
            var selectExpr = (this as ISelectWithModelExpression<TEntity, TResult>).SelectExpression();
            IQueryable<TResult> resquery = GetFilterQuery(conditions, extraWhereCondition, out var withOutPageingQuery).Select(selectExpr);
            var res = MapToDynamicFilterResult(conditions, resquery, withOutPageingQuery.Select(selectExpr));
            return res;
        }
        else
        {

            if ((mappingType == OutputModelMappingTypeEnum.Auto || mappingType == OutputModelMappingTypeEnum.UseMappingModel) && this is ISelectWithModelMapper<TEntity, TResult>)
            {
                var res = MapToDynamicFilterResult(conditions, GetFilterQuery(conditions, extraWhereCondition, out var withOutPageingQuery), withOutPageingQuery);

                var t = new DynamicQueryFilterResult<TResult>()
                {
                    Data = res.Data.Select(x => (this as ISelectWithModelMapper<TEntity, TResult>).MapEntityToOutput(x)).ToList(),
                    PageNumber = res.PageNumber,
                    PageSize = res.PageSize,
                    TotalCount = res.TotalCount
                };
                if (conditions?.DisablePaging == true)
                {
                    t.TotalCount = t.Data.Count;
                    t.PageSize = t.Data.Count;
                }
                return t;
            }
            else if (mappingType == OutputModelMappingTypeEnum.Auto || mappingType == OutputModelMappingTypeEnum.UseAutoMapper)
            {
                var q = GetFilterQuery(conditions, extraWhereCondition, out var withOutPageingQuery);
                var count = conditions?.DisablePaging == null ? conditions?.PageSize ?? 0 + 1 : withOutPageingQuery.Count();
                var t = new DynamicQueryFilterResult<TResult>()
                {
                    Data = _mapper.ProjectTo<TResult>(q).ToList(),
                    PageNumber = conditions?.PageNumber ?? 1,
                    PageSize = conditions?.PageSize ?? count,
                    TotalCount = count
                };
                if (conditions?.DisablePaging == true)
                {
                    t.TotalCount = t.Data.Count;
                    t.PageSize = t.Data.Count == 0 ? 10 : t.Data.Count;
                }
                return t;
            }
            else
            {
                var countTemp = 0;
                var res = GetFilterQuery(conditions, extraWhereCondition, out var withOutPageingQuery);
                try
                {
                    if (conditions?.DisablePaging != null)
                        countTemp = withOutPageingQuery.Count();
                }
                catch
                {
                }
                var count = conditions?.DisablePaging == null ? conditions?.PageSize ?? 0 + 1 : countTemp;
                var t = new DynamicQueryFilterResult<TResult>();

                t.Data = res.Select(x => _mapper.Map<TResult>(x)).ToList();
                t.PageNumber = conditions?.PageNumber ?? 1;
                t.PageSize = conditions?.PageSize ?? count;
                t.TotalCount = count;

                if (conditions?.DisablePaging == true)
                {
                    t.TotalCount = t.Data.Count;
                    t.PageSize = t.Data.Count == 0 ? 10 : t.Data.Count;
                    t.PageNumber = 1;
                }
                return t;
            }
        }
    }

    public Task<DynamicQueryFilterResult<TResult>> GetItemsByFilterAsync<TResult>(FilterAndSortConditions conditions, Expression<Func<TEntity, bool>> extraWhereCondition = null, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetItemsByFilter<TResult>(conditions, extraWhereCondition, mappingType));
    }

    //ToDO Move it to repository 
    private IQueryable<TEntity> GetFilterQuery(FilterAndSortConditions filter, Expression<Func<TEntity, bool>> extraWhereCondition, out IQueryable<TEntity> withoutPaging)
    {
        IQueryable<TEntity> beforesort = _repository.GetAll();

        var filterExpr = extraWhereCondition;
        //TODO Add Language filter in futures
        if (filter != null && !string.IsNullOrEmpty(filter.WhereConditionText))
        {
            //TO DO Add filter Text
        }
        if (filterExpr != null)
        {
            beforesort = beforesort.Where(filterExpr);
        }
        else
        {
            beforesort = beforesort.AsQueryable();
        }
        //TO DO Add Sort 
        var sortedQuery = beforesort;

        withoutPaging = beforesort;
        if (filter == null || filter.DisablePaging == true)
            return sortedQuery;
        else
            return sortedQuery.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
    }

    protected DynamicQueryFilterResult<TResult> MapToDynamicFilterResult<TResult>(PageParams pageParams, IQueryable<TResult> query, IQueryable<TResult> withoutPagingQuery)
    {
        if (pageParams == null)
            pageParams = new PageParams() { DisablePaging = true };
        if (query == null)
        {
            query = pageParams.DisablePaging ?? false ? withoutPagingQuery : withoutPagingQuery.Skip((pageParams.PageNumber - 1) * pageParams.PageSize).Take(pageParams.PageSize);
        }
        List<TResult> resList = query.ToList();
        var res = new DynamicQueryFilterResult<TResult>
        {
            Data = resList
        };
        if (pageParams != null)
        {
            res.PageNumber = pageParams.PageNumber;
            res.PageSize = pageParams.DisablePaging == true ? resList.Count == 0 ? 10 : resList.Count : pageParams.PageSize;
        }
        res.TotalCount = pageParams.DisablePaging == null ? pageParams.PageSize + 1 : pageParams.DisablePaging == true ? resList.Count : withoutPagingQuery.Count();
        return res;
    }
    private TEntity GetEntityFromInputModel(InputModel inputModel, OutputModelMappingTypeEnum mappingType)
    {
        if (mappingType == OutputModelMappingTypeEnum.UseExpression)
            throw new InvalidOperationException("Can not use Exression mode in input");
        if (mappingType == OutputModelMappingTypeEnum.UseMappingModel && !(this is IInputModelCustomMapper<TEntity, InputModel>))
            throw new InvalidOperationException("This class dos not contain CustomMapping implemented");
        TEntity entity;
        if (this is IInputModelCustomMapper<TEntity, InputModel> && mappingType != OutputModelMappingTypeEnum.UseAutoMapper)
            entity = (this as IInputModelCustomMapper<TEntity, InputModel>).MapInputToEnity(inputModel);
        else
            entity = _mapper.Map<TEntity>(inputModel);
        return entity;
    }

    private OutputModel GetOutputModelFromEntity(TEntity entity, OutputModelMappingTypeEnum mappingType)
    {
        if (mappingType == OutputModelMappingTypeEnum.UseExpression)
            throw new InvalidOperationException("Can not use Exression mode in input");
        if (mappingType == OutputModelMappingTypeEnum.UseMappingModel && !(this is ISelectWithModelMapper<TEntity, OutputModel>))
            throw new InvalidOperationException("This class dos not contain CustomMapping implemented");
        OutputModel output;
        if (this is ISelectWithModelMapper<TEntity, InputModel> && mappingType != OutputModelMappingTypeEnum.UseAutoMapper)
            output = (this as ISelectWithModelMapper<TEntity, OutputModel>).MapEntityToOutput(entity);
        else
            output = _mapper.Map<OutputModel>(entity);
        return output;
    }

    public OutputModel Insert(InputModel inputModel, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        TEntity entity = GetEntityFromInputModel(inputModel, mappingType);
        entity = _repository.InsertWithSaveChange(entity);
        return GetOutputModelFromEntity(entity, mappingType);
    }

    public async Task<OutputModel> InsertAsync(InputModel inputModel, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto, CancellationToken cancellationToken = default)
    {
        TEntity entity = GetEntityFromInputModel(inputModel, mappingType);
        entity = await _repository.InsertWithSaveChangeAsync(entity, cancellationToken);
        return GetOutputModelFromEntity(entity, mappingType);
    }

    public Task RemoveAsync(object id, CancellationToken cancellationToken = default)
    {
        return _repository.RemoveWithSaveChangeAsync(id, cancellationToken);
    }

    public void Update(InputModel inputModel, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto)
    {
        TEntity entity = GetEntityFromInputModel(inputModel, mappingType);
        _repository.UpdateWithSaveChange(entity);
    }

    public Task<int> UpdateAsync(InputModel inputModel, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto, CancellationToken cancellationToken = default)
    {
        TEntity entity = GetEntityFromInputModel(inputModel, mappingType);
        return _repository.UpdateWithSaveChangeAsync(entity, cancellationToken);
    }
}
