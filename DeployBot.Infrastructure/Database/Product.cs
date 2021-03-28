using LiteDB;

namespace DeployBot.Infrastructure.Database
{
    public class Product : IEntity<Product>
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }

        public void EnsureIndices(ILiteCollection<Product> collection)
        {
            collection.EnsureIndex(p => p.Name);
        }
    }
}