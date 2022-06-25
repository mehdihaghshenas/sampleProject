using MAction.BaseServices;
using MAction.SampleOnion.Domain.Entity.SAL;
using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;

namespace MAction.SampleOnion.Service.Company
{
    public interface ICompanyServiceWithExpression:IBaseService<SaleCompany, SaleCompanyInputModel, SaleCompanyOutputModel>
    {
    }
}