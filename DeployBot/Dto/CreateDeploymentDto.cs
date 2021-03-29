using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeployBot.Dto
{
    public class CreateDeploymentDto
    {
        [FromForm(Name = "version")]
        public string Version { get; set; }

        [FromForm(Name = "release_zip")]
        public IFormFile ReleaseZip { get; set; }
    }

    public class CreateApplicationDto
    {
        public string Name { get; set; }
    }
}
