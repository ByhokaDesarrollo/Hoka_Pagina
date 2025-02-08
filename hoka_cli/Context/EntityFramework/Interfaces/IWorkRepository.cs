using System.Collections.Generic;

namespace hoka_cli.Context.EntityFramework.Interfaces
{
    public interface IWorkRepository<TEntity> where TEntity : class
    {
        void InsertEntity(TEntity entity);

        void InsertEntities(IEnumerable<TEntity> entities);

        void UpdateEntity(TEntity entity);

        void UpdateEntities(IEnumerable<TEntity> entities);

        void DeleteEntity(TEntity entity);

        void DeleteEntities(IEnumerable<TEntity> entities);

        void SaveChanges();
    }
}
