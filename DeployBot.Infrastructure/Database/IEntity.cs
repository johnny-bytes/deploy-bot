using LiteDB;

namespace DeployBot.Infrastructure.Database
{
    public interface IEntity<TEntity>
    {
        ObjectId Id { get; set; }
        void EnsureIndices(ILiteCollection<TEntity> collection);
    }
}
