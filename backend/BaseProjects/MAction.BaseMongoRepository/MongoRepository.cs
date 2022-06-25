using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseClasses.Helpers;
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

public interface IMongoRepository<TEntity> : IBaseRepository<TEntity> where TEntity : IBaseEntity
{
}

public class MongoRepository<T> : IMongoRepository<T> where T : IBaseEntity
{
    private readonly IMongoClient _mongoClient;
    private IMongoDatabase _db;
    private readonly IMongoCollection<T> _collection;
    private IClientSessionHandle? _session = null;

    protected virtual string CollectionName
    {
        get => typeof(T).Name;
    }

    public MongoRepository(IMongoDependencyProvider databaseName, IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
        _db = _mongoClient.GetDatabase(databaseName.DatabaseName);
        _collection = _db.GetCollection<T>(CollectionName);
    }

    public T Get(object id)
    {
        return _collection.Find(ExpressionHelpers.GetIdFilter<T>(id)).FirstOrDefault();
    }

    public IQueryable<T> GetAll()
    {
        var languagePropertyInfo =
            typeof(T).GetProperties().FirstOrDefault(x => x.PropertyType == typeof(LanguageEnum));
        if (languagePropertyInfo != null)
        {
            var condition = ExpressionHelpers.GetConstantExpressionFromType<T>(languagePropertyInfo,
                Enum.Parse<LanguageEnum>(
                    CultureInfo.CurrentCulture.Name, ignoreCase: true));
            return _collection.AsQueryable().Where(condition);
        }
        else
            return _collection.AsQueryable();
    }

    public Task<T> GetAsync(object id, CancellationToken cancellationToken = default)
    {
        return _collection.Find(ExpressionHelpers.GetIdFilter<T>(id)).FirstOrDefaultAsync(cancellationToken);
    }

    public void Insert(T entity)
    {
        CheckExistAndAddId(entity);

        SetLanguagePropertyInfo(entity);

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

        SetLanguagePropertyInfo(entity);

        if (_session == null)
            _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        // Begin transaction
        if (!_session.IsInTransaction)
            _session.StartTransaction();

        await _collection.InsertOneAsync(_session, entity, cancellationToken: cancellationToken);
        return;
    }
    private static bool CheckExistAndAddId(T entity, bool add = true)
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
                if (add)
                    entity.SetPrimaryKeyValue(Guid.NewGuid().ToString());
                return false;
            }
            else
                return true;
        }
        else if (entity.GetPrimaryKeyType() == typeof(Guid))
        {
            if (((Guid)entity.GetPrimaryKeyValue()) == Guid.Empty)
            {
                if (add)
                    entity.SetPrimaryKeyValue(Guid.NewGuid());
                return false;
            }
            else
                return true;
        }
        else if (entity.GetPrimaryKeyType() == typeof(ObjectId))
        {
            if (((ObjectId)entity.GetPrimaryKeyValue()) == ObjectId.Empty)
            {
                if (add)
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
        SetLanguagePropertyInfo(entity);

        _collection.InsertOne(entity);
        return entity;
    }

    public async Task<T> InsertWithSaveChangeAsync(T entity, CancellationToken cancellationToken = default)
    {
        SetLanguagePropertyInfo(entity);

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

    public void RemoveWithSaveChange(object id)
    {
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(id);
        var res = _collection.DeleteOne(lambda);
        if (res.DeletedCount != 1)
            throw new NotFoundException("Enity dos not have id");
    }

    public async Task<int> RemoveWithSaveChangeAsync(object id, CancellationToken cancellationToken = default)
    {
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(id);
        var res = await _collection.DeleteOneAsync(lambda);
        if (res.DeletedCount != 1)
            throw new NotFoundException("Enity dos not have id");
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
    private void ExceludePropertyFromUpdate(T entity)
    {
        //TODO Excelude Not Be updated properties
    }
    public void Update(T entity)
    {
        if (!CheckExistAndAddId(entity))
            throw new InvalidEntityException("Enity dos not have id");

        SetLanguagePropertyInfo(entity);
        ExceludePropertyFromUpdate(entity);

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

        ExceludePropertyFromUpdate(entity);
        SetLanguagePropertyInfo(entity);

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
        if (!CheckExistAndAddId(entity, false))
            throw new InvalidEntityException("Enity dos not have id");
        ExceludePropertyFromUpdate(entity);
        SetLanguagePropertyInfo(entity);

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

        ExceludePropertyFromUpdate(entity);
        SetLanguagePropertyInfo(entity);

        object keyValue = entity.GetPrimaryKeyValue();
        Expression<Func<T, bool>> lambda = ExpressionHelpers.GetIdFilter<T>(keyValue);
        var res = await _collection.ReplaceOneAsync(lambda, entity, cancellationToken: cancellationToken);
        if (res.ModifiedCount != 1)
            throw new NotFoundException("Enity dos not have id");
        else
            return 1;
    }

    private static void SetLanguagePropertyInfo(T entity)
    {
        var languagePropertyInfo =
            typeof(T).GetProperties().FirstOrDefault(x => x.PropertyType == typeof(LanguageEnum));

        if (languagePropertyInfo == null) return;

        var value = (LanguageEnum)(languagePropertyInfo.GetValue(entity) ?? throw new InvalidOperationException());
        if ((int)value == 0)
            languagePropertyInfo.SetValue(entity, Enum.Parse<LanguageEnum>(
                CultureInfo.CurrentCulture.Name, ignoreCase: true));
    }
}