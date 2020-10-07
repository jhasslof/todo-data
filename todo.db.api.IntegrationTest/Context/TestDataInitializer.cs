using System;
using System.Collections.Generic;
using System.Text;
using todo.db.Models;

namespace todo.db.api.IntegrationTest.Context
{
    class TestDataInitializer
    {
        private static readonly object _lock = new object();

        public static List<TodoItem> GetSeedingTodoItems() => new List<TodoItem>() {
                 new TodoItem { Name = "Go shopping", IsComplete = false},
                 new TodoItem { Name = "feed cat", IsComplete = false }
            };


        internal static void InitializeDbForTests(ITodoDbTestContext db)
        {
            lock (_lock)
            {
                db.Seed<TodoItem>(GetSeedingTodoItems());
            }
        }

        public static void ReinitializeDbForTests(ITodoDbTestContext db)
        {
            // While a lock is held, the thread that holds the lock can again acquire and release the lock,
            // so should be ok to call the InitializeDbForTests() with the same lock.
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement
            lock (_lock)
            {
                db.Clean<TodoItem>();
                InitializeDbForTests(db);
            }
        }
    }
}
