using System;
using LiteDB;

namespace DeployBot.Infrastructure.Database
{
    public class Deployment : IEntity<Deployment>
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId ApplicationId { get; set; }
        public DateTime StatusChangedOn { get; set; }
        public DeploymentStatus Status { get; set; }
        public string Version { get; set; }

        public void EnsureIndices(ILiteCollection<Deployment> collection)
        {
            collection.EnsureIndex(d => d.ApplicationId);
        }
    }
}
