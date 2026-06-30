using GithubManager.Controllers;
using GithubManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace GithubManager.Tests
{
    public class UnitTest2
    {
        [Fact]
        public async Task UpdateArchiveStatus_ReturnsBadRequest_WhenRepositoryNameIsEmpty()
        {
            var settings = new Dictionary<string, string?>
            {
                { "Github:Token", "fake-token" },
                { "Github:Organization", "fake-org" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var logger = NullLogger<RepositoriesController>.Instance;
            var controller = new RepositoriesController(configuration, logger);

            var request = new ArchiveRequest
            {
                Archived = true
            };

            var result = await controller.UpdateArchiveStatus("", request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
