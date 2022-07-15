using AutoMapper;
using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
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
    private readonly IServiceDependencyProvider _dependencyProvider;

    public CompanyServiceWithExpression(IBaseRepositoryInt<SaleCompany> repository, IMapper mapper, IServiceDependencyProvider dependencyProvider) : base(repository, mapper, dependencyProvider)
    {
        _dependencyProvider=dependencyProvider;
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

    public override Expression<Func<SaleCompany, bool>> GetSelectPermissionExpression()
    {
        return x=>true;
    }
    public override void CheckInsertPermission(SaleCompany entity)
    {
        if(!_dependencyProvider.HasToken)
            throw new ForbiddenExpection("Forbid");
    }
    public override void CheckDeletePermission(SaleCompany entity)
    {
        if(!_dependencyProvider.HasToken)
            throw new ForbiddenExpection("Forbid");
    }
    public override void CheckUpdatePermission(SaleCompany entity)
    {
        if(!_dependencyProvider.HasToken)
            throw new ForbiddenExpection("Forbid");

    }
}
