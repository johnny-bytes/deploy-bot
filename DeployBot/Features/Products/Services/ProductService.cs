using System.Collections.Generic;
using DeployBot.Infrastructure.Database;
using LiteDB;

namespace DeployBot.Features.Products.Services
{
    public class ProductService
    {
        private readonly LiteDbRepository<Product> _dbContext;

        public ProductService(LiteDbRepository<Product> dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Product> GetAll()
        {
            return _dbContext.GetAll();
        }

        public Product GetById(ObjectId id)
        {
            return _dbContext.GetById(id);
        }

        public Product GetByName(string productName)
        {
            return _dbContext.Query()
                .Where(p => p.Name == productName)
                .First();
        }

        public Product GetByNameOrDefault(string productName)
        {
            return _dbContext.Query()
                .Where(p => p.Name == productName)
                .FirstOrDefault();
        }

        public Product GetOrAddByName(string productName)
        {
            var product = GetByNameOrDefault(productName);
            if (product == null)
            {
                product = AddProductWithName(productName);
            }

            return product;
        }

        private Product AddProductWithName(string productName)
        {
            var product = new Product
            {
                Name = productName
            };

            _dbContext.AddOrUpdate(product);
            return product;
        }
    }
}