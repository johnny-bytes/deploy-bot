using System;
using System.Collections.Generic;
using DeployBot.Infrastructure.Database;
using LiteDB;

namespace DeployBot.Features.Deployments.Services
{
    public class DeploymentLogService
    {
        private readonly LiteDbRepository<DeploymentLogEntry> _dbContext;

        public DeploymentLogService(LiteDbRepository<DeploymentLogEntry> dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<DeploymentLogEntry> GetLogs(ObjectId deploymentId)
        {
            return _dbContext.Query()
            .Where(dl => dl.DeploymentId == deploymentId)
            .ToList();
        }

        public DeploymentLogEntry CreateLogEntry(ObjectId deploymentId, string logText)
        {
            var deploymentLogEntry = new DeploymentLogEntry
            {
                Date = DateTime.UtcNow,
                DeploymentId = deploymentId,
                LogText = logText
            };

            _dbContext.AddOrUpdate(deploymentLogEntry);
            return deploymentLogEntry;
        }

        public int ClearLogs(ObjectId deploymentId)
        {
            return _dbContext.RemoveAll(dl => dl.DeploymentId == deploymentId);
        }
    }
}
