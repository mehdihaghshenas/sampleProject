using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;
using MAction.SipOnline.Service;

namespace MAction.SampleOnion.Service.Category;

public interface
    ICategoryService : IBaseServiceInt<Domain.Entity.SAL.Category, CategoryInputModel, CategoryOutputModel>
{
}