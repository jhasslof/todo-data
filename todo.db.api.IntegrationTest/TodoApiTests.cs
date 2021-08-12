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
using System.Text;
using System;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.EventHandlers;

namespace todo.db.api.IntegrationTest
{

    public class TodoApiTests : IClassFixture<IntegrationTestWebApplicationFactory<todo.db.api.Startup>>
    {
        private readonly IntegrationTestWebApplicationFactory<todo.db.api.Startup> _factory;

        public TodoApiTests(IntegrationTestWebApplicationFactory<todo.db.api.Startup> factory)
        {
            _factory = factory;
        }

        public static List<TodoItem> GetSeedingTodoItems() => new List<TodoItem>() {
                 new TodoItem { TodoItemId = 1001, Name = "Go shopping", IsComplete = false},
                 new TodoItem { TodoItemId = 1002, Name = "feed cat", IsComplete = false }
            };

        private HttpClient GetWebHostClientWithDefaultData()
        {
            var testFactoryWithSeedData = _factory.WithWebHostBuilder(builder =>
                 {
                     builder.ConfigureServices(services =>
                     {
                         var sp = services.BuildServiceProvider();
                         var db = sp.GetService<ITodoDbContext>() as ITodoDbTestContext;
                         TestDataInitializer.ReinitializeDbForTests(db, GetSeedingTodoItems(), identityInsert:true);
                     });
                 }
             );

            return testFactoryWithSeedData.CreateClient();
        }

        private HttpClient GetWebHostClientWithDataset(IEnumerable<TodoItem> todoItems)
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var sp = services.BuildServiceProvider();
                    var db = sp.GetService<ITodoDbContext>() as ITodoDbTestContext;
                    TestDataInitializer.ReinitializeDbForTests(db, todoItems);
                });
            }).CreateClient();
        }

        //

        [Theory]
        [InlineData("/api/todoitems")]
        [InlineData("/api/todoitems/1001")]
        [InlineData("/api/todoitems/1002")]
        public async Task TodoItems_GetApiEndpoint_ReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = GetWebHostClientWithDefaultData();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task TodoItems_GetItems_ReturnItemsFromDb()
        {
            //Arrange
            HttpClient client = GetWebHostClientWithDataset(new List<TodoItem> {
                        new TodoItem {Name = "make dinner"},
                        new TodoItem {Name = "set table"}
                    });

            //Act
            var response = await client.GetAsync("/api/todoitems");
            
            //Assert
            var todoItems = JsonConvert.DeserializeObject<TodoItemDTO[]>(await response.Content.ReadAsStringAsync());
            Assert.Equal(2, todoItems.Count());
            Assert.Contains(todoItems, t => t.Name == "make dinner");
            Assert.Contains(todoItems, t => t.Name == "set table");
        }

        [Fact]
        public async Task TodoItems_GetSpecificItem_ReturnsCorrectItemFromDb()
        {
            //Arrange
            var client = GetWebHostClientWithDataset(new List<TodoItem> {
                        new TodoItem {Name = "make dinner"},
                        new TodoItem {Name = "set table"}
                    });
            var response = await client.GetAsync("/api/todoitems");
            var todoItems = JsonConvert.DeserializeObject<TodoItemDTO[]>(await response.Content.ReadAsStringAsync());
            var dinnerItemId = todoItems.Single(t => t.Name == "make dinner").Id;

            //Act
            var dinnerItemResponse = await client.GetAsync($"/api/todoitems/{dinnerItemId}");

            //Assert
            var dinnerItem = JsonConvert.DeserializeObject<TodoItemDTO>(await dinnerItemResponse.Content.ReadAsStringAsync());
            Assert.Equal(dinnerItemId, dinnerItem.Id);
            Assert.Equal("make dinner", dinnerItem.Name);
            Assert.False(dinnerItem.IsComplete);
        }

        [Fact]
        public async Task TodoItems_AddItem_StoresAndReturnsItemFromDb()
        {
            //Arrange
            var client = _factory.CreateClient();
            var newTodoItem = new TodoItemDTO { Name = "do dishes", IsComplete = true };

            //Act
            var responsePost = await client.PostAsync("/api/todoitems",
                                                    new StringContent(JsonConvert.SerializeObject(newTodoItem),
                                                                        Encoding.UTF8,
                                                                        "application/json"));
            responsePost.EnsureSuccessStatusCode();

            //Assert
            var response = await client.GetAsync("/api/todoitems");
            var todoItems = JsonConvert.DeserializeObject<TodoItemDTO[]>(await response.Content.ReadAsStringAsync());
            var dishItem = todoItems.Single(t => t.Name == "do dishes");
            Assert.True(dishItem.IsComplete);
        }

        [Fact]
        public async Task TodoItems_UpdateItem_StoresUpdateAndReturnsItemFromDb()
        {
            //Arrange
            var client = GetWebHostClientWithDataset(new List<TodoItem> {
                        new TodoItem {Name = "make dinner"},
                    });

            //Act
            var response = await client.GetAsync("/api/todoitems");
            var todoItems = JsonConvert.DeserializeObject<TodoItemDTO[]>(await response.Content.ReadAsStringAsync());
            var dinnerItem = todoItems.Single(t => t.Name == "make dinner");
            var dinnerItemId = dinnerItem.Id;
            dinnerItem.Name = "make breakfast";
            dinnerItem.IsComplete = true;

            var responsePost = await client.PutAsync($"/api/todoitems/{dinnerItem.Id}",
                                                    new StringContent(JsonConvert.SerializeObject(dinnerItem),
                                                                        Encoding.UTF8,
                                                                        "application/json"));
            responsePost.EnsureSuccessStatusCode();

            //Assert
            var breakfastItemResponse = await client.GetAsync($"/api/todoitems/{dinnerItemId}");
            var breakfastItem = JsonConvert.DeserializeObject<TodoItemDTO>(await breakfastItemResponse.Content.ReadAsStringAsync());
            Assert.Equal(dinnerItemId, breakfastItem.Id);
            Assert.Equal("make breakfast", breakfastItem.Name);
            Assert.True(breakfastItem.IsComplete);
        }
    }
}
