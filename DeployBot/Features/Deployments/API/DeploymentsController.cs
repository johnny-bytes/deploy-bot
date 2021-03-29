using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeployBot.Features.Authentication;
using DeployBot.Features.Deployments.DTO;
using DeployBot.Features.Deployments.Services;
using DeployBot.Infrastructure.Database;
using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeployBot.Features.Deployments.API
{
    [Authorize(AuthenticationSchemes = BasicAuthenticationOptions.DefaultScheme)]
    [Route("api/deployments")]
    [ApiController]
    public class DeploymentsController : ControllerBase
    {
        private readonly DeploymentService _deploymentService;

        public DeploymentsController(DeploymentService deploymentService)
        {
            _deploymentService = deploymentService;
        }

        [HttpGet("{productName}")]
        public ActionResult<IEnumerable<DeploymentDto>> GetDeployments([FromRoute] string productName)
        {
            var deployments = _deploymentService.GetDeploymentsByProduct(productName);

            return Ok(deployments.Select(d => new DeploymentDto
            {
                StatusChangedOn = d.StatusChangedOn,
                Status = d.Status,
                Version = d.Version
            }).ToList());
        }

        [HttpPost("{productName}")]
        public async Task<ActionResult<DeploymentDto>> CreateDeployment([FromRoute] string productName, [FromForm] CreateDeploymentDto creaeteReleaseDto)
        {
            var deployment = await _deploymentService.CreateDeployment(productName, creaeteReleaseDto);

            return Ok(new DeploymentDto
            {
                Id = deployment.Id.ToString(),
                Version = deployment.Version
            });
        }

        [HttpPost("enqueue")]
        public IActionResult EnqueueDeployment([FromBody] EnqueueDeploymentDto dto)
        {
            var deployment = _deploymentService.GetById(dto.Id);
            if (deployment == null)
            {
                return NotFound();
            }

            _deploymentService.UpdateStatus(deployment, DeploymentStatus.Enqueued);

            return Ok();
        }
    }
}
