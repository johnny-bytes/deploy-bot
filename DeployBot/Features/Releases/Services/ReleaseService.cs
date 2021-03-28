using DeployBot.Features.Products.Services;
using DeployBot.Features.Releases.DTO;
using DeployBot.Features.Shared.Services;
using DeployBot.Infrastructure.Database;
using LiteDB;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DeployBot.Features.Releases.Services
{
    public class ReleaseService
    {
        private readonly LiteDbRepository<Release> _dbContext;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly ProductService _productService;

        public ReleaseService(LiteDbRepository<Release> dbContext,
        ServiceConfiguration serviceConfiguration, ProductService productService)
        {
            _dbContext = dbContext;
            _serviceConfiguration = serviceConfiguration;
            _productService = productService;
        }

        public IEnumerable<Release> GetReleasesByProduct(string productName)
        {
            var product = _productService.GetByName(productName);

            return _dbContext.Query()
                .Where(d => d.ProductId == product.Id)
                .ToList();
        }

        public Release GetReleaseById(ObjectId id)
        {
            return _dbContext.GetById(id);
        }

        public Release GetReleaseByVersion(string version)
        {
            return _dbContext.Query()
                .Where(r => r.Version == version)
                .FirstOrDefault();
        }

        public async Task<Release> CreateOrUpdateRelease(string productName, CreateReleaseDto createReleaseDto)
        {
            var product = _productService.GetOrAddByName(productName);

            var releaseDropOff = _serviceConfiguration.GetReleaseDropOffFolder(product.Name, createReleaseDto.Version);
            if (Directory.Exists(releaseDropOff))
            {
                Directory.Delete(releaseDropOff, true);
            }

            Directory.CreateDirectory(releaseDropOff);
            using (var fileStream = File.OpenWrite(Path.Combine(releaseDropOff, "release.zip")))
            using (var readStream = createReleaseDto.ReleaseZip.OpenReadStream())
            {
                await readStream.CopyToAsync(fileStream);
            }

            var release = _dbContext.Query().Where(r => r.Version == createReleaseDto.Version).FirstOrDefault() ?? new Release
            {
                ProductId = product.Id,
                Version = createReleaseDto.Version
            };

            _dbContext.AddOrUpdate(release);

            return release;
        }
    }
}
