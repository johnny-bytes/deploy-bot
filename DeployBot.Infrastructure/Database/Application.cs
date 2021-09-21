using LiteDB;

namespace DeployBot.Infrastructure.Database
{
    public class Application : IEntity<Application>
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }

        public void EnsureIndices(ILiteCollection<Application> collection)
        {
            collection.EnsureIndex(p => p.Name);
        }
    }
}