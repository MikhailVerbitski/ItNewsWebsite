using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Data.Contracts
{
    public interface IRepository<T> where T : class
    {
        T Create(T entity);
        T Read(Expression<Func<T, bool>> keySelector, params Expression<Func<T, object>>[] includes);
        T Read(string keySelector, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> ReadMany(Expression<Func<T, bool>>[] whereProperties, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> ReadMany(string where, string orderBy, params Expression<Func<T, object>>[] includes);
        void Update(T entity, params Expression<Func<T, object>>[] properties);
        void Delete(T entity);
    }
}
