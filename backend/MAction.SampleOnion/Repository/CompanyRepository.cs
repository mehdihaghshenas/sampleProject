using MAction.BaseClasses;
using MAction.BaseMongoRepository;
using MAction.SampleOnion.Domain.Entity.SAL;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.SampleOnion.Repository;

internal class CompanyRepository : BaseRepositoryInt<SaleCompany>, ICompanyRepository
{
    public CompanyRepository(IMongoDependencyProvider databaseName, IMongoClient mongoClient, IBaseServiceDependencyProvider baseServiceDependencyProvider) : 
    base(databaseName, mongoClient, baseServiceDependencyProvider)
    {
    }
}

internal interface ICompanyRepository:IBaseRepositoryInt<SaleCompany>
{
}
