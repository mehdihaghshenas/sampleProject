using AutoMapper;
using MAction.BaseClasses;
using MAction.SampleOnion.Domain.Entity.SAL;
using MAction.SampleOnion.Service.ViewModel.Input;

namespace MAction.SampleOnion.Service.ViewModel.Output
{
    public class SaleCompanyOutputModel : BaseDTO<SaleCompany, SaleCompanyOutputModel>
    {
        public string Name { get; set; }
        public int Id { get; set; }


        public override void CustomReverseMapping(IMappingExpression<SaleCompany, SaleCompanyOutputModel> mapping)
        {
            mapping.ForMember(x => x.Name, m => m.MapFrom(x => x.CompanyName));
        }

    }
}