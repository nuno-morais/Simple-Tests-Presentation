using System.Net;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Repositories;
using BillTracker.Tests.Fixtures;
using BillTracker.Tests.Helpers;
using Xunit;

namespace BillTracker.Tests.Acceptance
{
    public class UpdateBillTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly DatabaseFixture _fixture;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public UpdateBillTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _fixture = factory.DatabaseFixture;

            var options = _fixture.GetDbContextOptions<BillsContext>();
            BillsContext context = new BillsContext(options);

            context.Bills.AddRange(
                new Bill() { Description = "Bill1", Price = 100 },
                new Bill() { Description = "Bill2", Price = 200 },
                new Bill() { Description = "Bill3", Price = 300 },
                new Bill() { Description = "Bill4", Price = 400 },
                new Bill() { Description = "Bill5", Price = 500 }
            );
            context.SaveChanges();
        }

        [Fact]
        public async Task Update_EndpointReturnsSuccessAndReturnTheBill()
        {
            var client = _factory.CreateClient();

            var body = new
            {
                Id = 2,
                Description = "Dummy",
                Price = 10
            };
            var response = await client.PutAsync("/Bills/2", ContentHelper.GetStringContent(body));

            var result = await response.Content.ReadAsStringAsync();
            var bill = ContentHelper.ReadFromString<Bill>(result);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(new Bill() { Id = 2, Description = "Dummy", Price = 10 }, bill);
        }

        [Fact]
        public async Task Update_InvalidBill_EndpointReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var body = new
            {
                Id = 2,
                Price = 10
            };
            var response = await client.PutAsync("/Bills/2", ContentHelper.GetStringContent(body));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_NonExistentBill_EndpointReturnsNotFound()
        {
            var client = _factory.CreateClient();

            var body = new
            {
                Id = 20,
                Description = "Dummy",
                Price = 10
            };
            var response = await client.PutAsync("/Bills/20", ContentHelper.GetStringContent(body));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
