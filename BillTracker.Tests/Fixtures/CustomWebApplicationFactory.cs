using System;
using System.Linq;
using BillTracker.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BillTracker.Tests.Fixtures
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private DatabaseFixture _databaseFixture;
        public DatabaseFixture DatabaseFixture {
            get {
                if (_databaseFixture == null)
                {
                    _databaseFixture = new DatabaseFixture();
                    _databaseFixture.InitializeAsync().Wait();
                }
                return _databaseFixture;
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BillsContext>));

                services.Remove(descriptor);

                services.AddDbContext<BillsContext>(options =>
                {
                    options.UseSqlServer(DatabaseFixture.ConnectionString);
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<BillsContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();

                    // DatabaseFixture.ResetDatabaseAsync().Wait();
                }
            });
        }
    }
}
