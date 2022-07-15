using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using System.Linq;
using MAction.BaseClasses;

namespace MAction.BaseServices;

public interface IBaseService<TKey,TEntity, InputModel, OutputModel> where TEntity : BaseEntity where TKey : new()
{
    void SetHasSystemPrivilege(bool value);

    DynamicQueryFilterResult<TEntity> GetItemsByFilter(FilterAndSortConditions filter, Expression<Func<TEntity, bool>> extraWhereCondition = null);
    Task<DynamicQueryFilterResult<TEntity>> GetItemsByFilterAsync(FilterAndSortConditions filter, Expression<Func<TEntity, bool>> extraWhereCondition = null,
        CancellationToken cancellationToken = default);
    public DynamicQueryFilterResult<TResult> GetItemsByFilter<TResult>(FilterAndSortConditions conditions, Expression<Func<TEntity, bool>> extraWhereCondition = null,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto);
    public Task<DynamicQueryFilterResult<TResult>> GetItemsByFilterAsync<TResult>(FilterAndSortConditions conditions, Expression<Func<TEntity, bool>> extraWhereCondition = null,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto, CancellationToken cancellationToken = default);

    public DynamicQueryFilterResult<OutputModel> GetItemByFilter(FilterAndSortConditions conditions, Expression<Func<TEntity, bool>> extraWhereCondition = null,
    OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto);
    public Task<DynamicQueryFilterResult<OutputModel>> GetItemByFilterAsync(FilterAndSortConditions conditions, Expression<Func<TEntity, bool>> extraWhereCondition = null,
        OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto, CancellationToken cancellationToken = default);
    OutputModel Get(TKey entityId, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto);
    Task<OutputModel> GetAsync(TKey entityId, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto, CancellationToken cancellationToken = default);
    void Update(InputModel inputModel, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto);
    Task<int> UpdateAsync(InputModel inputModel, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto,
        CancellationToken cancellationToken = default);
    OutputModel Insert(InputModel inputModel, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto);
    Task<OutputModel> InsertAsync(InputModel inputModel, OutputModelMappingTypeEnum mappingType = OutputModelMappingTypeEnum.Auto,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(TKey id, CancellationToken cancellationToken = default);

    TEntity Insert(TEntity entity);
    Task<TEntity> InsertAsync(TEntity entity);
    void Update(TEntity entity);
    Task<int> UpdateAsync(TEntity entity);
    
    //TO DO Add Bulk Insert


}
