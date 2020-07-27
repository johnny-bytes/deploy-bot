using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

namespace DeployBot.Infrastructure.Database
{
    public class Deployment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey(nameof(Release))]
        public int ReleaseId { get; set; }
        public Release Release { get; set; }

        public DateTime DeployedOn { get; set; }

        public DeploymentStatus Status { get; set; }
    }

    public enum DeploymentStatus
    {
        Success = 0,
        Failed = 1
    }
}
