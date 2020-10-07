using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using todo.db.api.IntegrationTest.Context;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using todo.db.Models;
using System.Collections.Generic;
using todo.db.api.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace todo.db.api.IntegrationTest
{
    public class TodoApiTests : IClassFixture<IntegrationTestWebApplicationFactory<todo.db.api.Startup>>
    {
        private readonly IntegrationTestWebApplicationFactory<todo.db.api.Startup> _factory;

        public TodoApiTests(IntegrationTestWebApplicationFactory<todo.db.api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/todoitems")]
        [InlineData("/api/todoitems/1")]
        public async Task Get_ApiEndpoint_ReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task TodoItem_GetItems_ReturnItemsFromDb()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var sp = services.BuildServiceProvider();
                    var db = sp.GetService<ITodoDbContext>() as ITodoDbTestContext;
                    db.Clean<TodoItem>();
                    db.Seed<TodoItem>( new List<TodoItem> { 
                        new TodoItem {Name = "make dinner"},
                        new TodoItem {Name = "set table"}
                    });
                });
            }).CreateClient();

            var response = await client.GetAsync("/api/todoitems");
            var todoItems = JsonConvert.DeserializeObject<TodoItemDTO[]>(await response.Content.ReadAsStringAsync());
            Assert.Equal(2, todoItems.Count());
            Assert.Contains(todoItems, t => t.Name == "make dinner");
            Assert.Contains(todoItems, t => t.Name == "set table");
        }
    }
}
