using LiteDB;

namespace DeployBot.Infrastructure.Database
{
    public class Release : IEntity<Release>
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Version { get; set; }
        public ObjectId ProductId { get; set; }

        public void EnsureIndices(ILiteCollection<Release> collection)
        {
            collection.EnsureIndex(r => r.Version);
        }
    }
}
