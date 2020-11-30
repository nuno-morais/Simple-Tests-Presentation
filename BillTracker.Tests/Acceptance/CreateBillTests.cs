using System.Net;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Tests.Fixtures;
using BillTracker.Tests.Helpers;
using Xunit;

namespace BillTracker.Tests.Acceptance
{
    public class CreateBillTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public CreateBillTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Create_EndpointReturnsSuccessAndReturnTheBill()
        {
            var client = _factory.CreateClient();

            var body = new
            {
                Description = "Dummy",
                Price = 10
            };
            var response = await client.PostAsync("/Bills", ContentHelper.GetStringContent(body));

            var result = await response.Content.ReadAsStringAsync();
            var bill = ContentHelper.ReadFromString<Bill>(result);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(new Bill() { Id = 1, Description = "Dummy", Price = 10 }, bill);
        }

        [Fact]
        public async Task Create_InvalidBill_EndpointReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var body = new
            {
                Price = 10
            };
            var response = await client.PostAsync("/Bills", ContentHelper.GetStringContent(body));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
