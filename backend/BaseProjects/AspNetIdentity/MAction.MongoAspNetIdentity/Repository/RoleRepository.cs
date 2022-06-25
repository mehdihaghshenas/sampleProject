using MAction.BaseMongoRepository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAction.AspNetIdentity.Mongo.Domain;

namespace MAction.AspNetIdentity.Mongo.Repository
{
    public class RoleRepository : MongoRepository<ApplicationRole>, IRoleRepository
    {
        protected override string CollectionName => "Roles";
        public RoleRepository(IMongoDependencyProvider databaseName, IMongoClient mongoClient) : base(databaseName, mongoClient)
        {
        }

    }

    public interface IRoleRepository : IMongoRepository<ApplicationRole>
    {
    }
}
