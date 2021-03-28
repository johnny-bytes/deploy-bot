using LiteDB;

namespace DeployBot.Infrastructure.Database
{
    public interface IEntity<TEntity>
    {
        void EnsureIndices(ILiteCollection<TEntity> collection);
    }
}
