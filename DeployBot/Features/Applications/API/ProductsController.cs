using System.Collections.Generic;
using System.Linq;
using DeployBot.Features.Authentication;
using DeployBot.Features.Applications.DTO;
using DeployBot.Features.Applications.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeployBot.Features.Applications.API
{

    [Authorize(AuthenticationSchemes = BasicAuthenticationOptions.DefaultScheme)]
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