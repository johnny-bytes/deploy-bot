using System;
using DeployBot.Infrastructure.Database;

namespace DeployBot.Features.Deployments.DTO
{
    public class DeploymentDto
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public DateTime DeployedOn { get; set; }
        public DeploymentStatus Status { get; set; }
    }
}
