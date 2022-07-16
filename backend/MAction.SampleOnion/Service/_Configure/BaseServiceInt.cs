using AutoMapper;
using MAction.BaseClasses;

namespace MAction.SipOnline.Service;
public interface IBaseServiceInt<TEntity, TInputModel, TOutputModel> : BaseServices.IBaseService<int, TEntity, TInputModel, TOutputModel>
where TEntity : BaseEntity
{
}

public class BaseServiceInt<TEntity, TInputModel, TOutputModel> : BaseServices.BaseServiceWithKey<int, TEntity, TInputModel, TOutputModel>, IBaseServiceInt<TEntity, TInputModel, TOutputModel>
    where TEntity : BaseEntity
{
    public BaseServiceInt(IBaseRepository<TEntity, int> repository, IMapper mapper, IBaseServiceDependencyProvider baseServiceDependencyProvider) : base(repository, mapper, baseServiceDependencyProvider)
    {
    }
}
