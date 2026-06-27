using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GithubManager.Models;

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
            var token = _configuration["Github:Token"];
            var organization = _configuration["Github:Organization"];

            using var client = new HttpClient();

            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("GithubManager", "1.0"));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(
                $"https://api.github.com/user/repos?visibility=all");

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
                archived = repo.GetProperty("archived").GetBoolean(),
                topics = repo.TryGetProperty("topics", out var topicsElement)
                    ? topicsElement.EnumerateArray()
                        .Select(topic => topic.GetString())
                        .Where(topic => topic != null)
                        .ToList()
                    : new List<string>()
            }).ToList();

            return Ok(repositories);
        }

        [HttpPatch("{repositoryName}/archive")]
        public async Task<IActionResult> UpdateArchiveStatus(string repositoryName, [FromBody] ArchiveRequest request)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
            {
                return BadRequest("Repository name is required.");
            }

            var token = _configuration["Github:Token"];
            var organization = _configuration["Github:Organization"];

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("GitHub token is missing.");
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("GithubManager", "1.0"));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var payload = JsonSerializer.Serialize(new
            {
                archived = request.Archived
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PatchAsync(
                $"https://api.github.com/repos/{organization}/{repositoryName}",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, new
                {
                    message = "Failed to update repository archive status.",
                    githubResponse = errorBody
                });
            }

            return Ok(new
            {
                message = request.Archived ? "Repository archived." : "Repository unarchived."
            });
        }
    }
}