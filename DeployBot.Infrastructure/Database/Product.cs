using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeployBot.Infrastructure.Database
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [InverseProperty(nameof(Release.Product))]
        public ICollection<Release> Releases { get; set; }

        [InverseProperty(nameof(Deployment.Product))]
        public ICollection<Deployment> Deployments { get; set; }
    }
}