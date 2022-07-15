
using MAction.BaseClasses;
using MAction.BaseMongoRepository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MAction.SampleOnion.Repository;
public class BaseRepositoryInt<T> : MongoRepository<T, int>, IBaseRepositoryInt<T> where T : IBaseEntity
{
    public BaseRepositoryInt(IMongoDependencyProvider databaseName, IMongoClient mongoClient, IBaseServiceDependencyProvider baseServiceDependencyProvider):
    base(databaseName,mongoClient,baseServiceDependencyProvider)
    {
        
    }
}

public interface IBaseRepositoryInt<T> : IMongoRepository<T, int> where T : IBaseEntity
{
}

public class BaseRepositoryObjectId<T> : MongoRepository<T, ObjectId>, IBaseRepositoryObjectId<T> where T : IBaseEntity
{
    public BaseRepositoryObjectId(IMongoDependencyProvider databaseName, IMongoClient mongoClient, IBaseServiceDependencyProvider baseServiceDependencyProvider):
    base(databaseName,mongoClient,baseServiceDependencyProvider)
    {
        
    }
}

public interface IBaseRepositoryObjectId<T> : IMongoRepository<T, ObjectId> where T : IBaseEntity
{
}
