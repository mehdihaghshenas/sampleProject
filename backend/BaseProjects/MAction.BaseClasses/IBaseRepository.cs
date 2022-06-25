
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MAction.BaseClasses
{
    public interface IBaseRepository<T> where T : IBaseEntity
    {
        IQueryable<T> GetAll();
        T Get(object id);
        Task<T> GetAsync(object id, CancellationToken cancellationToken = default);
        void Insert(T entity);
        Task InsertAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        void Remove(T entity);
        Task RemoveAsync(T entity, CancellationToken cancellationToken = default);
        void SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        T InsertWithSaveChange(T entity);
        Task<T> InsertWithSaveChangeAsync(T entity, CancellationToken cancellationToken = default);
        void UpdateWithSaveChange(T entity);
        Task<int> UpdateWithSaveChangeAsync(T entity, CancellationToken cancellationToken = default);
        void RemoveWithSaveChange(object id);
        Task<int> RemoveWithSaveChangeAsync(object id, CancellationToken cancellationToken = default);
        //TO DO Add Bulk Insert
        //TO DO Add Check Privillage

    }
}
