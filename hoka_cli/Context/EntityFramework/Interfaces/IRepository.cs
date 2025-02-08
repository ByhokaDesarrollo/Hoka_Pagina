using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace hoka_cli.Context.EntityFramework.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();

        TEntity Get(int id);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity SingleOrDefaultByQuery(string query);

        TEntity FirstOrDefaultByQuery(string query);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> FindByQuery(string query);
    }
}
