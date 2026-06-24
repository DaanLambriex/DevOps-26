using GithubManager.Models;

namespace GithubManager.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ArchiveRequest_ShouldStoreArchivedValue()
        {
            var request = new ArchiveRequest
            {
                Archived = true
            };

            Assert.True(request.Archived);
        }
    }
}
