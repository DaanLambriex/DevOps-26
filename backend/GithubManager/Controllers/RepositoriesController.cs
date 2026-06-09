using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GithubManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoriesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RepositoriesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetRepositories()
        {
            var token = _configuration["GitHub:Token"];
            var organization = _configuration["GitHub:Organization"];

            using var client = new HttpClient();

            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("GithubManager", "1.0"));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(
                $"https://api.github.com/users/{organization}/repos");

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, new
                {
                    message = "GitHub API request failed",
                    statusCode = (int)response.StatusCode,
                    reason = response.ReasonPhrase,
                    githubResponse = errorBody
                });
            }

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var repositories = document.RootElement.EnumerateArray().Select(repo => new
            {
                name = repo.GetProperty("name").GetString(),
                description = repo.GetProperty("description").GetString(),
                htmlUrl = repo.GetProperty("html_url").GetString(),
                isPrivate = repo.GetProperty("private").GetBoolean(),
                archived = repo.GetProperty("archived").GetBoolean()
            }).ToList();

            return Ok(repositories);
        }
    }
}