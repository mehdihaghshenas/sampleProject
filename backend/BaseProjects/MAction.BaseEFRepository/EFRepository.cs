using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseClasses.Helpers;
using MAction.BaseClasses.Language;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MAction.BaseEFRepository;
public interface IEFRepository<TEntity,TKey> : IBaseRepository<TEntity,TKey> where TEntity : class, IBaseEntity
{
}


public class EFRepository<T, TKey> : IEFRepository<T, TKey> where T : class, IBaseEntity
{
    private readonly DbContext _context;
    internal DbSet<T> entities;
    private readonly IBaseServiceDependencyProvider _baseServiceDependencyProvider;
    public EFRepository(DbContext context, IBaseServiceDependencyProvider baseServiceDependencyProvider)
    {
        _context = context;
        entities = context.Set<T>();
        _baseServiceDependencyProvider = baseServiceDependencyProvider;
    }
    public void SetHasSystemPrivilege(bool value)
    {
        _baseServiceDependencyProvider.SetInternalMode(value);
    }


    public T Get(TKey id)
    {

        var expression = ExpressionHelpers.GetIdFilter<T>(id);
        return entities.Where(expression).FirstOrDefault();
    }

    public IQueryable<T> GetAll()
    {
        Expression<Func<T, bool>>? langCondition = LanguageHelpers.GetLanguageExpressionCondition<T>();
        if (langCondition != null)
            return entities.AsQueryable().Where(langCondition);
        else
            return entities.AsQueryable();
    }

    public Task<T> GetAsync(object id, CancellationToken cancellationToken = default)
    {
        var expression = ExpressionHelpers.GetIdFilter<T>(id);
        return entities.Where(expression).FirstOrDefaultAsync(cancellationToken);
    }

    private void AddRequiredFieldForInsert(T entity)
    {
        //TODO Check system Privillage and user info and Timezone 
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        LanguageHelpers.SetLanguagePropertyInfo(entity);

        if (entity.GetType().IsSubclassOf(typeof(BaseEntityWithCreationInfo)))
        {
            (entity as BaseEntityWithCreationInfo).CreateAt = _baseServiceDependencyProvider.HasSystemPrivilege() ? DateTimeOffset.UtcNow : TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _baseServiceDependencyProvider.TimeZoneConverterService.GetClientTimeZoneInfo());
            (entity as BaseEntityWithCreationInfo).TimeZone = _baseServiceDependencyProvider.TimeZoneConverterService.GetClientTimeZoneInfo().StandardName; // TODO Get from basedependency provider
            (entity as BaseEntityWithCreationInfo).UserCreationId = _baseServiceDependencyProvider.UserId != null ? _baseServiceDependencyProvider.UserId.ToString() : null; //TODO Get From base dependency
        }
        //For Check Permission in add with out savechange we have create a pipeline to test on save change
    }

    public void Insert(T entity)
    {
        AddRequiredFieldForInsert(entity);
        entities.Add(entity);
    }


    public Task InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        AddRequiredFieldForInsert(entity);
        return entities.AddAsync(entity, cancellationToken).AsTask();
    }

    public T InsertWithSaveChange(T entity)
    {
        bool hasoldtran = _context.Database.CurrentTransaction != null;
        var transaction = _context.Database.CurrentTransaction ?? _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        try
        {
            Insert(entity);
            SaveChanges();

            //check permission

            if (!hasoldtran)
                transaction.Commit();
        }
        catch (UnauthorizedException)
        {
            if (!hasoldtran)
                transaction.Rollback();
            throw;
        }
        catch (Exception)
        {
            if (!hasoldtran)
                transaction.Rollback();
            throw;
        }
        return entity;
    }

    public async Task<T> InsertWithSaveChangeAsync(T entity, CancellationToken cancellationToken = default)
    {
        bool hasoldtran = _context.Database.CurrentTransaction != null;
        var transaction = _context.Database.CurrentTransaction ?? _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        try
        {
            await InsertAsync(entity, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            //TODO Check permission
            if (!hasoldtran)
                transaction.Commit();
        }
        catch (UnauthorizedException)
        {
            if (!hasoldtran)
                transaction.Rollback();
            throw;
        }
        catch (Exception)
        {
            if (!hasoldtran)
                transaction.Rollback();
            throw;
        }
        return entity;
    }

    public void Remove(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        //TODO Check permission

        AttachIfNotAttached(entity);
        entities.Remove(entity);
    }

    public Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        Remove(entity);
        return Task.CompletedTask;
    }

    public void RemoveWithSaveChange(TKey id)
    {
        var e = Get(id);
        Remove(e);
        SaveChanges();
    }

    public async Task<int> RemoveWithSaveChangeAsync(object id, CancellationToken cancellationToken = default)
    {
        var e = await GetAsync(id, cancellationToken);
        await RemoveAsync(e, cancellationToken);
        return await SaveChangesAsync(cancellationToken);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private void AttachIfNotAttached(T entity)
    {
        LanguageHelpers.SetLanguagePropertyInfo(entity);

        if (_context.Entry<T>(entity).State != EntityState.Modified)
        {
            _context.Attach<T>(entity);
        }
        _context.Entry<T>(entity).State = !CheckExistAndAddId(entity, false) ? EntityState.Added : EntityState.Modified;
        if (typeof(BaseEntityWithCreationInfo).IsAssignableFrom(typeof(T)))
        {
            //we allways set CreateAt isModified = false because this column is computed column and update in sql server
            _context.Entry<T>(entity).Property(x => (x as BaseEntityWithCreationInfo).CreateAt).IsModified = false;

            if (_context.Entry<T>(entity).State == EntityState.Modified)
            {
                //be careful that some columns can not edit at all like CreateAt
                _context.Entry<T>(entity).Property(x => (x as BaseEntityWithCreationInfo).TimeZone).IsModified = false;
                _context.Entry<T>(entity).Property(x => (x as BaseEntityWithCreationInfo).UserCreationId).IsModified = false;
            }
        }
    }
    private static bool CheckExistAndAddId(T entity, bool add = true)
    {
        if (entity.GetPrimaryKeyType() == typeof(int))
        {
            var id = (int)entity.GetPrimaryKeyValue();
            if (id > 0)
                return true;
            else
                return false;
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
        else
            throw new NotImplementedException("Not Allowed");
    }

    public void Update(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        AttachIfNotAttached(entity);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Update(entity);
        return Task.CompletedTask;
    }

    public void UpdateWithSaveChange(T entity)
    {
        Update(entity);
        SaveChanges();
    }

    public async Task<int> UpdateWithSaveChangeAsync(T entity, CancellationToken cancellationToken = default)
    {
        await UpdateAsync(entity, cancellationToken);
        try
        {
            return await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
    }
}

//TODO in insert update delete check HasSystem Privilege if true save with systemUser and Can set with 