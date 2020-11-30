using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Xunit;
using Respawn;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Tests.Fixtures
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly Checkpoint _emptyDatabaseCheckpoint;

        private const string BaseConnectionString = "Data Source=127.0.0.1,1433;User=sa;Password=Password123;";

        private readonly string _dbName;

        public string ConnectionString { get; }

        public DatabaseFixture()
        {
            _dbName = "integration-tests-for-bills" + "-" + Guid.NewGuid().ToString();

            _emptyDatabaseCheckpoint = new Checkpoint
                {
                    // Reseed identity columns
                    WithReseed = true,
                    TablesToIgnore = new[]
                    {
                        // DbUp journal does not need cleaning
                        "SchemaVersions"
                    }
                };

            var builder = new SqlConnectionStringBuilder(BaseConnectionString)
            {
                InitialCatalog = _dbName
            };

            ConnectionString = builder.ToString();
        }

        public async Task InitializeAsync()
        {
            using var connection = new SqlConnection(BaseConnectionString);
            await connection.OpenAsync();

            await ExecuteDbCommandAsync(connection, $"CREATE DATABASE [{_dbName}]");
            await ExecuteDbCommandAsync(connection, $"USE [{_dbName}]");
            await ExecuteDbCommandAsync(connection, $"CREATE TABLE Bills(" +
                $"Id INT NOT NULL IDENTITY, " +
                $"Description TEXT NOT NULL, " +
                $"Price INT NOT NULL, " +
                $"PRIMARY KEY (Id)" +
                $");");
        }

        public async Task DisposeAsync()
        {
            using var connection = new SqlConnection(BaseConnectionString);
            await connection.OpenAsync();

            await ExecuteDbCommandAsync(connection, $"EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{_dbName}'");
            await ExecuteDbCommandAsync(connection, "USE [master]");
            await ExecuteDbCommandAsync(connection, $"ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            await ExecuteDbCommandAsync(connection, "USE [master]");
            await ExecuteDbCommandAsync(connection, $"DROP DATABASE [{_dbName}]");
        }

        public async Task ResetDatabaseAsync()
        {
            await _emptyDatabaseCheckpoint.Reset(ConnectionString);
        }

        private async Task ExecuteDbCommandAsync(SqlConnection connection, string commandText)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = commandText;
            await cmd.ExecuteNonQueryAsync();
        }

        public DbContextOptions<T> GetDbContextOptions<T>() where T: DbContext
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlServer(ConnectionString)
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
