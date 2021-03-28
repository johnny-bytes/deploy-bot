using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeployBot.Features.Authentication;
using DeployBot.Features.Deployments.DTO;
using DeployBot.Features.Deployments.Services;
using DeployBot.Features.Releases.Services;
using LiteDB;
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
        private readonly ReleaseService _releaseService;

        public DeploymentsController(DeploymentService deploymentService, ReleaseService releaseService)
        {
            _deploymentService = deploymentService;
            _releaseService = releaseService;
        }

        [HttpGet("{productName}")]
        public ActionResult<IEnumerable<DeploymentDto>> GetDeployments([FromRoute] string productName)
        {
            var deployments = _deploymentService.GetDeploymentsByProduct(productName);

            return Ok(deployments.Select(d => new DeploymentDto
            {
                StatusChangedOn = d.StatusChangedOn,
                Status = d.Status,
                Version = _releaseService.GetReleaseById(d.ReleaseId).Version
            }).ToList());
        }

        [HttpPost("enqueue")]
        public async Task<ActionResult> CreateDeployment([FromBody] EnqueueDeploymentDto dto)
        {
            var release = _releaseService.GetReleaseByVersion(dto.Version);
            if (release == null)
            {
                return NotFound();
            }

            await _deploymentService.EnqueueDeploymentForRelease(release);

            return Ok();
        }
    }
}
