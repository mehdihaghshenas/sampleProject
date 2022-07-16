
using MAction.BaseClasses;
using MAction.BaseMongoRepository;
using MongoDB.Driver;

namespace MAction.SampleOnion.Repository;

public class BaseRepositoryInt<T> : MongoRepository<T, int>, IBaseRepositoryInt<T> where T : IBaseEntity
{
    public interface IBaseRepositoryInt<T> : IMongoRepository<T, int> where T : IBaseEntity
    {
    }

    public BaseRepositoryInt(IMongoDependencyProvider databaseName, IMongoClient mongoClient, IBaseServiceDependencyProvider baseServiceDependencyProvider):
    base(databaseName,mongoClient,baseServiceDependencyProvider)
    {
        
    }
}
