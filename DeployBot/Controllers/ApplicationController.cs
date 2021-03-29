using System.Collections.Generic;
using System.Linq;
using DeployBot.Authentication;
using DeployBot.Dto;
using DeployBot.Features.Applications.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeployBot.Features.Deployments.Services;
using System.Threading.Tasks;
using DeployBot.Infrastructure.Database;
using LiteDB;

namespace DeployBot.Controllers
{

    [Authorize(AuthenticationSchemes = BasicAuthenticationOptions.DefaultScheme)]
    [Route("api/applications")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationService _applicationService;
        private readonly DeploymentService _deploymentService;
        private readonly DeploymentLogService _deploymentLogService;

        public ApplicationController(ApplicationService productService, DeploymentService deploymentService, DeploymentLogService deploymentLogService)
        {
            _applicationService = productService;
            _deploymentService = deploymentService;
            _deploymentLogService = deploymentLogService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ApplicationDto>> GetAllApplications()
        {
            return Ok(_applicationService.GetAll()
                .Select(app => new ApplicationDto
                {
                    Id = app.Id.ToString(),
                    Name = app.Name
                }));
        }

        [HttpPost]
        public ActionResult<ApplicationDto> CreateApplications([FromBody] CreateApplicationDto createApplicationDto)
        {
            var application = _applicationService.GetByNameOrDefault(createApplicationDto.Name);
            if (application != null)
            {
                return BadRequest($"Application with name {createApplicationDto.Name} already exists.");
            }

            application = _applicationService.CreateProductWithName(createApplicationDto.Name);
            return new ApplicationDto
            {
                Id = application.Id.ToString(),
                Name = application.Name
            };
        }

        [HttpGet("{applicationId}/deployments")]
        public ActionResult<IEnumerable<DeploymentDto>> GetDeployments([FromRoute] string applicationId)
        {
            var deployments = _deploymentService.GetDeploymentsByApplicationId(new ObjectId(applicationId));

            return Ok(deployments.Select(d => new DeploymentDto
            {
                Id = d.Id.ToString(),
                Version = d.Version,
                Status = d.Status,
                StatusChangedOn = d.StatusChangedOn,
            }).ToList());
        }

        [HttpPost("{applicationId}/deployments")]
        public async Task<ActionResult<DeploymentDto>> CreateDeployment([FromRoute] string applicationId, [FromForm] CreateDeploymentDto createDeploymentDto)
        {
            ObjectId applicationObjectId = new ObjectId(applicationId);
            var application = _applicationService.GetById(applicationObjectId);
            if (application == null)
            {
                return BadRequest("Application was not found.");
            }

            if (_deploymentService.CheckVersionExists(applicationObjectId, createDeploymentDto.Version))
            {
                return BadRequest("Version already exists.");
            }

            var deployment = await _deploymentService.CreateDeployment(application, createDeploymentDto);
            return Ok(new DeploymentDto
            {
                Id = deployment.Id.ToString(),
                Version = deployment.Version,
                Status = deployment.Status,
                StatusChangedOn = deployment.StatusChangedOn
            });
        }

        [HttpDelete("{applicationId}/deployments/{deploymentId}")]
        public ActionResult DeleteDeployment([FromRoute] string applicationId, [FromRoute] string deploymentId)
        {
            ObjectId applicationObjectId = new ObjectId(applicationId);
            var application = _applicationService.GetById(applicationObjectId);
            if (application == null)
            {
                return BadRequest("Application was not found.");
            }

            var deployment = _deploymentService.GetById(new ObjectId(deploymentId));
            if (deployment == null)
            {
                return NotFound();
            }

            _deploymentService.DeleteDeployment(deployment);
            return Ok();
        }

        [HttpPost("{applicationId}/deployments/{deploymentId}/enqueue")]
        public IActionResult EnqueueDeployment([FromRoute] string deploymentId)
        {
            var deployment = _deploymentService.GetById(new ObjectId(deploymentId));
            if (deployment == null)
            {
                return BadRequest("Deployment was not found.");
            }

            _deploymentService.UpdateStatus(deployment, DeploymentStatus.Enqueued);

            return Ok();
        }

        [HttpGet("{applicationId}/deployments/{deploymentId}/logs")]
        public ActionResult<IEnumerable<DeploymentLogEntry>> GetLogs([FromRoute] string applicationId, [FromRoute] string deploymentId)
        {
            ObjectId applicationObjectId = new ObjectId(applicationId);
            var application = _applicationService.GetById(applicationObjectId);
            if (application == null)
            {
                return BadRequest("Application was not found.");
            }

            ObjectId deploymentObjectId = new ObjectId(deploymentId);
            var deployment = _deploymentService.GetById(deploymentObjectId);
            if (deployment == null)
            {
                return BadRequest("Deployment was not found.");
            }

            return Ok(_deploymentLogService.GetLogs(deploymentObjectId));
        }

        [HttpDelete("{applicationId}/deployments/{deploymentId}/logs")]
        public ActionResult<IEnumerable<DeploymentLogEntry>> ClearLogs([FromRoute] string applicationId, [FromRoute] string deploymentId)
        {
            ObjectId applicationObjectId = new ObjectId(applicationId);
            var application = _applicationService.GetById(applicationObjectId);
            if (application == null)
            {
                return BadRequest("Application was not found.");
            }

            ObjectId deploymentObjectId = new ObjectId(deploymentId);
            var deployment = _deploymentService.GetById(deploymentObjectId);
            if (deployment == null)
            {
                return BadRequest("Deployment was not found.");
            }

            _deploymentLogService.ClearLogs(deploymentObjectId);
            return Ok();
        }
    }
}