using MAction.BaseServices;
using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;

namespace MAction.SampleOnion.Service.Category;

public interface
    ICategoryService : IBaseService<Domain.Entity.SAL.Category, CategoryInputModel, CategoryOutputModel>
{
}