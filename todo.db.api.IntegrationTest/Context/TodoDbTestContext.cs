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

        IEnumerable<TModel> ITodoDbTestContext.List<TModel>() where TModel : class
        {
            Database.EnsureCreated();
            return Set<TModel>().ToArray();
        }

        bool ITodoDbTestContext.HasItems<TModel>() where TModel : class
        {
            Database.EnsureCreated();
            return Set<TModel>().Any();
        }

        void ITodoDbTestContext.Seed<TModel>(IEnumerable<TModel> seedTodoItems, bool identityInsert = false) where TModel : class
        {
            Database.EnsureCreated();
            if (identityInsert)
            {
                SeedWithIdentityInsertOn(seedTodoItems);
            }
            else
            {
                Set<TModel>().AddRange(seedTodoItems);
                SaveChanges();
            }
        }

        //Ref: https://entityframeworkcore.com/saving-data-identity-insert
        private void SeedWithIdentityInsertOn<TModel>(IEnumerable<TModel> seedTodoItems) where TModel : class
        {
            using (var seedContext = new TodoDbTestContext(_options))
            {
                seedContext.Set<TModel>().AddRange(seedTodoItems);
                seedContext.Database.OpenConnection();
                try
                {
                    seedContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Todo].[TodoItem] ON;");
                    seedContext.SaveChanges();
                    seedContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Todo].[TodoItem] OFF;");
                }
                finally
                {
                    seedContext.Database.CloseConnection();
                }
            }
        }

        void ITodoDbTestContext.Clean<TModel>() where TModel : class
        {
            Database.EnsureCreated();
            var itemsToRemove = Set<TModel>();
            if (itemsToRemove.Any())
            {
                Set<TModel>().RemoveRange(itemsToRemove);
                SaveChanges();
            }
        }
    }
}
