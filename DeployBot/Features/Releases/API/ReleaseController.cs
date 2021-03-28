using DeployBot.Features.Authentication;
using DeployBot.Features.Releases.DTO;
using DeployBot.Features.Releases.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeployBot.Features.Releases.API
{
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
    [Route("api/releases")]
    [ApiController]
    public class ReleaseController : ControllerBase
    {
        private readonly ReleaseService _releaseService;

        public ReleaseController(ReleaseService releaseService)
        {
            _releaseService = releaseService;
        }

        [HttpGet("{productName}")]
        public ActionResult<IEnumerable<ReleaseDto>> GetReleases([FromRoute] string productName)
        {
            var releases = _releaseService.GetReleasesByProduct(productName);

            return Ok(releases.Select(release => new ReleaseDto
            {
                Version = release.Version
            }));
        }

        [HttpPut("{productName}")]
        public async Task<ActionResult<ReleaseDto>> PutRelease([FromRoute] string productName, [FromForm] CreateReleaseDto creaeteReleaseDto)
        {
            var release = await _releaseService.CreateOrUpdateRelease(productName, creaeteReleaseDto);

            return Ok(new ReleaseDto
            {
                Id = release.Id.ToString(),
                Version = release.Version
            });
        }
    }
}
