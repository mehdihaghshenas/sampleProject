using MAction.BaseEFRepository;
using MAction.BaseMongoRepository;
using MAction.SampleOnion.Domain.Entity.SAL;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.SampleOnion.Repository
{
    internal class CompanyRepository : MongoRepository<SaleCompany>, ICompanyRepository
    {
        public CompanyRepository(IMongoDependencyProvider dbName, IMongoClient mongoClient) : base(dbName, mongoClient)
        {
        }
    }

    internal interface ICompanyRepository
    {
    }
}
