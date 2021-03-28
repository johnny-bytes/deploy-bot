using System.Collections.Generic;
using System.Linq;
using DeployBot.Features.Authentication;
using DeployBot.Features.Products.DTO;
using DeployBot.Features.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeployBot.Features.Products.API
{

    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProductDto>> GetAllProducts()
        {
            return Ok(_productService.GetAll()
                .Select(p => new ProductDto
                {
                    Name = p.Name
                }));
        }
    }
}