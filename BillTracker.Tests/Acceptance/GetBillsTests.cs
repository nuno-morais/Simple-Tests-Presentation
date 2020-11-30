using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BillTracker.Entities;
using BillTracker.Repositories;
using BillTracker.Tests.Fixtures;
using BillTracker.Tests.Helpers;
using Xunit;
using System.Linq;

namespace BillTracker.Tests.Acceptance
{
    public class GetBillsTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly DatabaseFixture _fixture;
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly IList<Bill> bills = new List<Bill>()
        {
            new Bill() { Description = "Bill1", Price = 100 },
            new Bill() { Description = "Bill2", Price = 200 },
            new Bill() { Description = "Bill3", Price = 300 },
            new Bill() { Description = "Bill4", Price = 400 },
            new Bill() { Description = "Bill5", Price = 500 }
        };

        public GetBillsTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _fixture = factory.DatabaseFixture;

            var options = _fixture.GetDbContextOptions<BillsContext>();
            var context = new BillsContext(options);

            context.Bills.AddRange(bills);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetBills_EndpointReturnsSuccessAndReturnTheBills()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Bills");

            var result = await response.Content.ReadAsStringAsync();
            var receivedBills = ContentHelper.ReadFromString<IList<Bill>>(result);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var index = 1;
            var expected = bills.Select(b => new Bill() { Id = index++, Description = b.Description, Price = b.Price });
            Assert.Equal(expected, receivedBills);
        }

    }
}
