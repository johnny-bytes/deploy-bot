using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeployBot.Infrastructure.Database
{
    public class Release
    {
        [Key]
        public int Id { get; set; }

        public string Version { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [InverseProperty(nameof(Deployment.Release))]
        public ICollection<Deployment> Releases { get; set; }
    }
}
