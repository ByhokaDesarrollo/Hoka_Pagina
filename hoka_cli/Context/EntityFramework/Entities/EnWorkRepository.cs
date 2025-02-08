using hoka_cli.Context.EntityFramework.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;

namespace hoka_cli.Context.EntityFramework.Entities
{
    public class EnWorkRepository<TEntity>
        : EnRepository<TEntity>,
          IWorkRepository<TEntity> where TEntity : class
    {
        internal DbSet<TEntity> _dbSet { get; private set; }

        public EnWorkRepository(DbContext context)
            : base(context)
        {
            _dbSet = _context.Set<TEntity>();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void InsertEntity(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void InsertEntities(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void UpdateEntity(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateEntities(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public void DeleteEntity(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteEntities(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
