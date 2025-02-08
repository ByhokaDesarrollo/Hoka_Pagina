using hoka_cli.Context.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace hoka_cli.Context.EntityFramework.Entities
{
    public class EnRepository<TEntity>
        : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;

        public EnRepository(DbContext context)
        {
            _context = context;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        public TEntity Get(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>()
                           .SingleOrDefault(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>()
                           .FirstOrDefault(predicate);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>()
                           .Where(predicate);
        }

        public TEntity SingleOrDefaultByQuery(string query)
        {
            return _context.Set<TEntity>()
                           .SqlQuery(query)
                           .SingleOrDefault();
        }

        public TEntity FirstOrDefaultByQuery(string query)
        {
            return _context.Set<TEntity>()
                           .SqlQuery(query)
                           .FirstOrDefault();
        }

        public IEnumerable<TEntity> FindByQuery(string query)
        {
            return _context.Set<TEntity>()
                           .SqlQuery(query)
                           .ToList();
        }
    }
}
