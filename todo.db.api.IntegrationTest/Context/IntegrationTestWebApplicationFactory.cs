using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace todo.db.api.IntegrationTest.Context
{
    public class IntegrationTestWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => 
                                    d.ServiceType == typeof(ITodoDbContext));

                services.Remove(descriptor);  //Removing default database context

                var dbName = "TodoDbForTest"; // + System.Threading.Thread.CurrentThread.ManagedThreadId;
                // Adding a custom test database context
                services.AddDbContext<ITodoDbContext, TodoDbTestContext>(options =>
                {
                    // Having a fixed name of the memory database will make all instances of TestFixtures use the same db.
                    options.UseInMemoryDatabase(dbName);
                });
                var sp = services.BuildServiceProvider();

                //TODO: Add custom logger
                // Data is configured for each test to be idempotent.
                //
                //using (var scope = sp.CreateScope())
                //{
                //    var scopedServices = scope.ServiceProvider;
                //    var db = scopedServices.GetRequiredService<ITodoDbContext>() as ITodoDbTestContext;
                //    var logger = scopedServices
                //        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                //    try
                //    {
                //        //Now database is ready to receive test data
                //        TestDataInitializer.InitializeDbForTests(db);
                //    }
                //    catch (Exception ex)
                //    {
                //        throw;
                //        logger.LogError(ex, "An error occurred seeding the " +
                //            "database with test messages. Error: {Message}", ex.Message);
                //    }
                //}
            });
        }
    }
}
