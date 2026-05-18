using Microsoft.AspNetCore.Mvc;

namespace GithubManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRepositories()
        {
            var repositories = new[]
            {
                new { name = "frontend", description = "Frontend repository" },
                new { name = "backend", description = "Backend repository" },
                new { name = "docs", description = "Documentation repository" }
            };

            return Ok(repositories);
        }
    }
}