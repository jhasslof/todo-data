using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo.db.Models;

namespace todo.db
{
    public class TodoDbContext : DbContext, ITodoDbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options)
        {
        }

        protected DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // https://docs.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations
            // https://docs.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations#default-values
            //By convention Id or TodoItemId is a store generated IDENTITY column.

            modelBuilder.Entity<TodoItem>()
                .Property(t => t.Created).HasDefaultValueSql("getdate()");
        }

        async Task<List<TodoItem>> ITodoDbContext.ListTodoItems()
        {
            return await TodoItems.ToListAsync();
        }

        async Task<TodoItem> ITodoDbContext.Get(long id)
        {
            return await TodoItems.FindAsync(id);
        }

        async Task ITodoDbContext.Update(TodoItem editItem)
        {
            var todoItem = await TodoItems.FindAsync(editItem.TodoItemId);
            if (todoItem == null)
            {
                throw new ApplicationException($"TodoItem id = {editItem.TodoItemId} not found");
            }

            try
            {
                await SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(editItem.TodoItemId))
            {
                throw new ApplicationException($"TodoItem id = {editItem.TodoItemId} not found");
            }
        }

        async Task<TodoItem> ITodoDbContext.Create(TodoItem newItem)
        {
            newItem.Created = DateTime.Now; //As Ef Core in memory db can't run SQL functions.
            TodoItems.Add(newItem); //Will generate id as added to db context
            await SaveChangesAsync(); 
            return newItem;
        }

        async Task ITodoDbContext.Delete(long id)
        {
            var itemToRemove = await TodoItems.FindAsync(id);
            if (itemToRemove == null)
            {
                throw new ApplicationException($"TodoItem id = {id} not found");
            }
            TodoItems.Remove(itemToRemove);
            await SaveChangesAsync();
        }

        private bool TodoItemExists(long id) =>
            TodoItems.Any(e => e.TodoItemId == id);
    }
}
