using AutoMapper;
using FluentValidation;
using MAction.BaseClasses;
using MAction.SampleOnion.Domain.Entity.SAL;

namespace MAction.SampleOnion.Service.ViewModel.Input;

public class SaleCompanyInputModel : BaseDTO<SaleCompany, SaleCompanyInputModel>
{
    public int Id { get; set; }
    public string Name { get; set; }

    public override void CustomReverseMapping(IMappingExpression<SaleCompany,SaleCompanyInputModel> mapping)
    {
        mapping.ForMember(x => x.Name, m => m.MapFrom(x => x.CompanyName));
    }
}
public class SaleCompanyValidator : AbstractValidator<SaleCompanyInputModel>
{
    public SaleCompanyValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage("NotNull");
    }
}
