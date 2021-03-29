using System.Collections.Generic;
using DeployBot.Infrastructure.Database;
using LiteDB;

namespace DeployBot.Features.Applications.Services
{
    public class ProductService
    {
        private readonly LiteDbRepository<Applications> _dbContext;

        public ProductService(LiteDbRepository<Applications> dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Applications> GetAll()
        {
            return _dbContext.GetAll();
        }

        public Applications GetById(ObjectId id)
        {
            return _dbContext.GetById(id);
        }

        public Applications GetByName(string productName)
        {
            return _dbContext.Query()
                .Where(p => p.Name == productName)
                .First();
        }

        public Applications GetByNameOrDefault(string productName)
        {
            return _dbContext.Query()
                .Where(p => p.Name == productName)
                .FirstOrDefault();
        }

        public Applications GetOrAddByName(string productName)
        {
            var product = GetByNameOrDefault(productName);
            if (product == null)
            {
                product = AddProductWithName(productName);
            }

            return product;
        }

        private Applications AddProductWithName(string productName)
        {
            var product = new Applications
            {
                Name = productName
            };

            _dbContext.AddOrUpdate(product);
            return product;
        }
    }
}