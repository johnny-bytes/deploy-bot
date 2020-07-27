using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeployBot.Features.Authentication;
using DeployBot.Features.Deployments.DTO;
using DeployBot.Features.Deployments.Services;
using DeployBot.Features.Releases.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeployBot.Features.Deployments.API
{
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
    [Route("api/deployments")]
    [ApiController]
    public class DeploymentsController : ControllerBase
    {
        private readonly DeploymentService _deploymentService;
        private readonly ReleaseDeploymentProcessor _releaseDeploymentProcessor;
        private readonly ReleaseService _releaseService;

        public DeploymentsController(DeploymentService deploymentService, ReleaseDeploymentProcessor releaseDeploymentProcessor, ReleaseService releaseService)
        {
            _deploymentService = deploymentService;
            _releaseDeploymentProcessor = releaseDeploymentProcessor;
            _releaseService = releaseService;
        }

        [HttpGet("{productName}")]
        public async Task<ActionResult<IEnumerable<DeploymentDto>>> GetDeployments([FromRoute] string productName)
        {
            var deployments = await _deploymentService.GetDeploymentsByProduct(productName);

            return Ok(deployments.Select(d => new DeploymentDto
            {
                DeployedOn = d.DeployedOn,
                Version = d.Release.Version
            }).ToList());
        }

        [HttpPost("create")]
        public async Task<ActionResult<DeploymentDto>> CreateDeployment([FromBody]CreateDeploymentDto dto)
        {
            var release = await _releaseService.GetReleaseById(dto.ReleaseId);
            if (release == null)
            {
                return NotFound();
            }

            var deployment = await _releaseDeploymentProcessor.RunAsync(release);
            return Ok(new DeploymentDto
            {
                Version = deployment.Release.Version,
                DeployedOn = deployment.DeployedOn,
                Id = deployment.Id,
                Status = deployment.Status
            });
        }
    }
}
