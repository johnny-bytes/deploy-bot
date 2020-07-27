using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeployBot.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DeployBot.Features.Deployments.Services
{
    public class DeploymentService
    {
        private readonly DeployBotDbContext _dbContext;

        public DeploymentService(DeployBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Deployment>> GetDeploymentsByProduct(string productName)
        {
            var product = await _dbContext.Products.FirstAsync(p => p.Name == productName);

            return _dbContext.Deployments
                .Include(d => d.Release)
                .Where(d => d.ProductId == product.Id);
        }

        public async Task<Deployment> CreateDeploymentForRelease(Release release, DateTime deployedOn, DeploymentStatus status)
        {
            var deployment = new Deployment
            {
                Release = release,
                Product = release.Product,
                DeployedOn = deployedOn,
                Status = status
            };

            await _dbContext.AddAsync(deployment);
            await _dbContext.SaveChangesAsync();

            return deployment;
        }
    }
}
