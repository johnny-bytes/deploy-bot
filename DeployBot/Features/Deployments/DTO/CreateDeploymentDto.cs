using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeployBot.Features.Deployments.DTO
{
    public class CreateDeploymentDto
    {
        [FromForm(Name = "version")]
        public string Version { get; set; }

        [FromForm(Name = "release_zip")]
        public IFormFile ReleaseZip { get; set; }
    }
}
