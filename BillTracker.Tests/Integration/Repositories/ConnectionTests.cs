using System.Data.SqlClient;
using System.Threading.Tasks;
using BillTracker.Tests.Fixtures;
using Xunit;

namespace BillTracker.Tests.Integration.Repositories
{
    public class ConnectionTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;

        private SqlConnection Connection { get; }

        public ConnectionTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            Connection = new SqlConnection(_fixture.ConnectionString);
        }

        public async Task InitializeAsync()
        {
            await Connection.OpenAsync();
        }

        public async Task DisposeAsync()
        {
            await Connection.DisposeAsync();
        }

        [Fact]
        public async Task Can_Connect()
        {
            using var command = Connection.CreateCommand();
            command.CommandText = "SELECT 1";

            var result = await command.ExecuteScalarAsync();

            Assert.Equal(1, result);
        }
    }
}
