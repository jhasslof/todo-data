using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using todo.db.Models;

namespace todo.db.api.IntegrationTest.Context
{
    public class TodoDbTestContext : TodoDbContext, ITodoDbTestContext
    {
        private DbContextOptions<TodoDbContext> _options;
        public TodoDbTestContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
            _options = options;
        }

        void ITodoDbTestContext.Seed<TModel>(IEnumerable<TModel> seedTodoItems) where TModel : class
        {
            Database.EnsureCreated();
            Set<TModel>().AddRange(seedTodoItems);
            SaveChanges();
        }

        void ITodoDbTestContext.Clean<TModel>() where TModel : class
        {
            var itemsToRemove = Set<TModel>();
            if (itemsToRemove.Any())
            {
                Set<TModel>().RemoveRange(itemsToRemove);
                SaveChanges();
            }
        }
    }
}
