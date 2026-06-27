using GithubManager.Controllers;
using GithubManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubManager.Tests
{
    public class UnitTest3
    {
        [Fact]
        public async Task UpdateArchiveStatus_ReturnsBadRequest_WhenGithubTokenIsMissing()
        {
            var settings = new Dictionary<string, string?>
            {
                { "Github:Organization", "fake-org" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var controller = new RepositoriesController(configuration);

            var request = new ArchiveRequest
            {
                Archived = true
            };

            var result = await controller.UpdateArchiveStatus("test-repo", request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
