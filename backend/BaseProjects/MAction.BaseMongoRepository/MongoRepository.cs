using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseClasses.Helpers;
using MAction.BaseClasses.Language;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MAction.BaseMongoRepository;

public interface IMongoRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : IBaseEntity
{
}

public class MongoRepository<T, TKey> : IMongoRepository<T, TKey> where T : IBaseEntity
{
    private readonly IBaseServiceDependencyProvider _baseServiceDependencyProvider;
    private readonly IMongoClient _mongoClient;
    private IMongoDatabase _db;
    private readonly IMongoCollection<T> _collection;
    private IClientSessionHandle? _session = null;

    protected virtual string CollectionName
    {
        get => typeof(T).Name;
    }

    public MongoRepository(IMongoDependencyProvider databaseName, IMongoClient mongoClient, IBaseServiceDependencyProvider baseServiceDependencyProvider)
    {
        _mongoClient = mongoClient;
        _db = _mongoClient.GetDatabase(databaseName.DatabaseName);
        _collection = _db.GetCollection<T>(CollectionName);
        _baseServiceDependencyProvider = baseServiceDependencyProvider;
    }
    public void SetHasSystemPrivilege(bool value)
    {
        _baseServiceDependencyProvider.SetInternalMode(value);
    }


    public T Get(TKey id)
    {
        return _collection.Find(ExpressionHelpers.GetIdFilter<T>(id)).FirstOrDefault();
    }

    public IQueryable<T> GetAll()
    {
        Expression<Func<T, bool>>? langCondition = LanguageHelpers.GetLanguageExpressionCondition<T>();
        if (langCondition != null)
            return _collection.AsQueryable().Where(langCondition);
        else
            return _collection.AsQueryable();
    }

#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
    public Task<T> GetAsync(object id, CancellationToken cancellationToken = default)
    {
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
        return _collection.Find(ExpressionHelpers.GetIdFilter<T>(id)).FirstOrDefaultAsync(cancellationToken);
    }

    private void SetRequiredDateForInsert(T entity)
    {
        entity.SetRequiredDateForInsert(_baseServiceDependencyProvider);
    }


    public void Insert(T entity)
    {
        CheckExistAndAddId(entity);

        SetRequiredDateForInsert(entity);

        if (_session == null)
            _session = _mongoClient.StartSession();
        // Begin transaction
        if (!_session.IsInTransaction)
            _session.StartTransaction();
        _collection.InsertOne(_session, entity);
    }

    public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        CheckExistAndAddId(entity);

        SetRequiredDateForInsert(entity);
        if (_session == null)
            _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        // Begin transaction
        if (!_session.IsInTransaction)
            _session.StartTransaction();

        await _collection.InsertOneAsync(_session, entity, cancellationToken: cancellationToken);
        return;
    }

    private static bool CheckExistAndAddId(T entity)
    {
        if (entity.GetPrimaryKeyType() == typeof(int))
        {
            var id = (int)entity.GetPrimaryKeyValue();
            if (id > 0)
                return true;
            throw new NotImplementedException("Should Get Max and add one also do in transaction");
        }
        else if (entity.GetPrimaryKeyType() == typeof(string))
        {
            var id = (string)entity.GetPrimaryKeyValue();
            if (string.IsNullOrWhiteSpace(id))
            {
                entity.SetPrimaryKeyValue(ObjectId.GenerateNewId().ToString());
                return false;
            }
            else
                return true;
        }
        else if (entity.GetPrimaryKeyType() == typeof(ObjectId))
        {
            if (((ObjectId)entity.GetPrimaryKeyValue()) == ObjectId.Empty)
            {
                entity.SetPrimaryKeyValue(ObjectId.GenerateNewId());
                return false;
            }
            else
                return true;
        }
        else if (entity.GetPrimaryKeyType() == typeof(ObjectId))
        {
            if (((ObjectId)entity.GetPrimaryKeyValue()) == ObjectId.Empty)
            {
                entity.SetPrimaryKeyValue(ObjectId.GenerateNewId());
                return false;
            }
            else
                return true;
        }
        else
            throw new NotImplementedException("Not Allowed");
    }


    public T InsertWithSaveChange(T entity)
    {
        SetRequiredDateForInsert(entity);

        _collection.InsertOne(entity);
        return entity;
    }

    public async Task<T> InsertWithSaveChangeAsync(T entity, CancellationToken cancellationToken = default)
    {
        SetRequiredDateForInsert(entity);

        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public void Remove(T entity)
    {
        if (!CheckExistAndAddId(entity))
            throw new InvalidEntityException("Enity dos not have id");
        object keyValue = entity.GetPrimaryKeyValue();
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(keyValue);
        if (_session == null)
            _session = _mongoClient.StartSession();
        // Begin transaction
        if (!_session.IsInTransaction)
            _session.StartTransaction();
        var res = _collection.DeleteOne(_session, lambda);
        if (res.DeletedCount != 1)
            throw new NotFoundException("Enity dos not have id");
    }

    public async Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (!CheckExistAndAddId(entity))
            throw new InvalidEntityException("Enity dos not have id");
        object keyValue = entity.GetPrimaryKeyValue();
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(keyValue);
        if (_session == null)
            _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        // Begin transaction
        if (!_session.IsInTransaction)
            _session.StartTransaction();
        var res = await _collection.DeleteOneAsync(_session, lambda, cancellationToken: cancellationToken);
        if (res.DeletedCount != 1)
            throw new NotFoundException("Enity dos not have id");
    }

    public void RemoveWithSaveChange(TKey id)
    {
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(id);
        var res = _collection.DeleteOne(lambda);
        if (res.DeletedCount != 1)
            throw new NotFoundException("Enity dos not have id");
    }

    public async Task<int> RemoveWithSaveChangeAsync(object id, CancellationToken cancellationToken = default)
    {
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(id);
        var res = await _collection.DeleteOneAsync(lambda, cancellationToken);
        if (res.DeletedCount != 1)
            throw new NotFoundException("Entity dos not have id");
        else
            return 1;
    }

    public void SaveChanges()
    {
        if (_session != null && _session.IsInTransaction)
            _session.CommitTransaction();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_session != null && _session.IsInTransaction)
        {
            _session.CommitTransactionAsync(cancellationToken);
            return Task.FromResult(1);
        }
        else
            return Task.FromResult(0);
    }

    public void Update(T entity)
    {
        if (!CheckExistAndAddId(entity))
            throw new InvalidEntityException("Enity dos not have id");

        LanguageHelpers.SetLanguagePropertyInfo(entity);

        object keyValue = entity.GetPrimaryKeyValue();
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(keyValue);
        if (_session == null)
            _session = _mongoClient.StartSession();
        // Begin transaction
        if (!_session.IsInTransaction)
            _session.StartTransaction();
        var res = _collection.ReplaceOne(_session, lambda, entity);
        if (res.ModifiedCount != 1)
            throw new NotFoundException("Enity dos not have id");
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (!CheckExistAndAddId(entity))
            throw new InvalidEntityException("Enity dos not have id");

        LanguageHelpers.SetLanguagePropertyInfo(entity);

        object keyValue = entity.GetPrimaryKeyValue();
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(keyValue);
        if (_session == null)
            _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        // Begin transaction
        if (!_session.IsInTransaction)
            _session.StartTransaction();
        var res = await _collection.ReplaceOneAsync(_session, lambda, entity, cancellationToken: cancellationToken);
        if (res.ModifiedCount != 1)
            throw new NotFoundException("Enity dos not have id");
    }

    public void UpdateWithSaveChange(T entity)
    {
        if (!CheckExistAndAddId(entity))
            throw new InvalidEntityException("Enity dos not have id");

        LanguageHelpers.SetLanguagePropertyInfo(entity);

        object keyValue = entity.GetPrimaryKeyValue();
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(keyValue);
        var res = _collection.ReplaceOne(lambda, entity);
        if (res.ModifiedCount != 1)
            throw new NotFoundException("Enity dos not have id");
    }

    public async Task<int> UpdateWithSaveChangeAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (!CheckExistAndAddId(entity))
            throw new InvalidEntityException("Enity dos not have id");

        LanguageHelpers.SetLanguagePropertyInfo(entity);

        object keyValue = entity.GetPrimaryKeyValue();
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(keyValue);
        var res = await _collection.ReplaceOneAsync(lambda, entity, cancellationToken: cancellationToken);
        if (res.ModifiedCount != 1)
            throw new NotFoundException("Enity dos not have id");
        else
            return 1;
    }
}