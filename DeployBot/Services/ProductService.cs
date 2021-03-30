using System.Collections.Generic;
using DeployBot.Infrastructure.Database;
using LiteDB;

namespace DeployBot.Features.Applications.Services
{
    public class ApplicationService
    {
        private readonly LiteDbRepository<Application> _dbContext;

        public ApplicationService(LiteDbRepository<Application> dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Application> GetAll()
        {
            return _dbContext.GetAll();
        }

        public Application GetByNameOrId(string applicationNameOrId)
        {
            return GetByNameOrDefault(applicationNameOrId) ?? GetById(new ObjectId(applicationNameOrId));
        }

        public Application GetById(ObjectId applicationNameOrId)
        {
            return _dbContext.GetById(new ObjectId(applicationNameOrId));
        }

        public Application GetByNameOrDefault(string applicationName)
        {
            return _dbContext.Query()
                .Where(p => p.Name == applicationName)
                .FirstOrDefault();
        }

        public Application GetOrAddByName(string applicationName)
        {
            var product = GetByNameOrDefault(applicationName);
            if (product == null)
            {
                product = CreateProductWithName(applicationName);
            }

            return product;
        }

        public Application CreateProductWithName(string applicationName)
        {
            var product = new Application
            {
                Name = applicationName
            };

            _dbContext.AddOrUpdate(product);
            return product;
        }
    }
}