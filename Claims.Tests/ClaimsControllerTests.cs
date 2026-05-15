using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;
using Xunit;

namespace Claims.Tests.OtherUnitTests
{
    public class ClaimsControllerTests
    {
        [Fact]
        public async Task Get_Claims()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});

            var client = application.CreateClient();

            var response = await client.GetAsync("/Claims");

            if (!response.IsSuccessStatusCode)
            {
                //Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                Debug.Assert(false, $"Request failed with status code: {response.StatusCode}");
            }

            //TODO: Apart from ensuring 200 OK being returned, what else can be asserted?
        }

    }
}
