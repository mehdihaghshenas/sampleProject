using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseMongoRepository;
using MAction.BaseServices;
using MAction.SampleOnion.Domain.Entity.SAL;
using MAction.SampleOnion.Repository;
using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;
using MAction.SipOnline.Service;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MAction.SampleOnion.Service.Company;

public class CompanyServiceWithExpression : BaseServiceInt<SaleCompany, SaleCompanyInputModel, SaleCompanyOutputModel>, ICompanyServiceWithExpression,
    ISelectWithModelExpression<SaleCompany, SaleCompanyOutputModel>,
    IInputModelCustomMapper<SaleCompany, SaleCompanyInputModel>
{
    public CompanyServiceWithExpression(IBaseRepositoryInt<SaleCompany> repository, IMapper mapper, IServiceDependencyProvider dependencyProvider) : base(repository, mapper, dependencyProvider)
    {
    }

    public SaleCompany MapInputToEnity(SaleCompanyInputModel inputModel)
    {
        return new()
        {
            CompanyName = inputModel.Name,
            Id = inputModel.Id
        };
    }

    public Expression<Func<SaleCompany, SaleCompanyOutputModel>> SelectExpression()
    {
        return x => new SaleCompanyOutputModel()
        {
            Name = x.CompanyName,
            Id = x.Id
        };
    }
}
