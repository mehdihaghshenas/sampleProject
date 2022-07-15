using MAction.BaseMongoRepository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAction.BaseClasses;
using Microsoft.AspNetCore.Identity;
using MAction.AspNetIdentity.Base.Entities;

namespace MAction.AspNetIdentity.Mongo.Repository
{
    public class RoleRepository<TRole, TKey> : MongoRepository<TRole, TKey>, IRoleRepository<TRole, TKey>
        where TRole : IdentityRole<TKey>, IRole, IBaseEntity, new()
        where TKey : IEquatable<TKey>
    {
        protected override string CollectionName => "Roles";
        public RoleRepository(IMongoDependencyProvider databaseName, IMongoClient mongoClient, IBaseServiceDependencyProvider baseServiceDependencyProvider) : base(databaseName, mongoClient, baseServiceDependencyProvider)
        {
        }

    }

    public interface IRoleRepository<TRole, TKey> : IMongoRepository<TRole, TKey>
        where TRole : IdentityRole<TKey>, IRole, IBaseEntity, new()
        where TKey : IEquatable<TKey>
    {
    }
}
