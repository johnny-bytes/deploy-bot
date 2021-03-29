using System.Collections.Generic;
using LiteDB;

namespace DeployBot.Infrastructure.Database
{
    public class LiteDbRepository<TEntity> where TEntity : IEntity<TEntity>
    {
        private LiteDatabase _liteDb;

        public LiteDbRepository(LiteDatabase liteDb)
        {
            _liteDb = liteDb;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Collection.FindAll();
        }

        public TEntity GetById(ObjectId id)
        {
            return Collection.FindById(id);
        }

        public ILiteQueryable<TEntity> Query()
        {
            return Collection.Query();
        }

        public void AddOrUpdate(TEntity entity)
        {
            Collection.Upsert(entity);
            entity.EnsureIndices(Collection);
        }

        public bool Remove(TEntity entity)
        {
            return Collection.Delete(entity.Id);
        }

        public int RemoveAll(System.Linq.Expressions.Expression<System.Func<TEntity, bool>> predicate)
        {
            return Collection.DeleteMany(predicate);
        }

        private ILiteCollection<TEntity> Collection => _liteDb.GetCollection<TEntity>();
    }
}