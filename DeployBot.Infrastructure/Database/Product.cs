using LiteDB;

namespace DeployBot.Infrastructure.Database
{
    public class Applications : IEntity<Applications>
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }

        public void EnsureIndices(ILiteCollection<Applications> collection)
        {
            collection.EnsureIndex(p => p.Name);
        }
    }
}