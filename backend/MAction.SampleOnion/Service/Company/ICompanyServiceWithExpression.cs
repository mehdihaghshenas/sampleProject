using MAction.SampleOnion.Domain.Entity.SAL;
using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;
using MAction.SipOnline.Service;

namespace MAction.SampleOnion.Service.Company
{
    public interface ICompanyServiceWithExpression:IBaseServiceInt<SaleCompany, SaleCompanyInputModel, SaleCompanyOutputModel>
    {
    }
}