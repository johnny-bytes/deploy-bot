using System;
using LiteDB;

namespace DeployBot.Infrastructure.Database
{
    public class DeploymentLogEntry : IEntity<DeploymentLogEntry>
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId DeploymentId { get; set; }
        public DateTime Date { get; set; }
        public string LogText { get; set; }

        public void EnsureIndices(ILiteCollection<DeploymentLogEntry> collection)
        {
            collection.EnsureIndex(dl => dl.DeploymentId);
        }
    }
}
