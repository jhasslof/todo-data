using System;
using System.Collections.Generic;
using System.Text;
using todo.db.Models;
using System.Linq;
using System.Threading.Tasks;

namespace todo.db.api.IntegrationTest.Context
{
    class TestDataInitializer
    {
        private static readonly object _lock = new object();

        internal static void InitializeDbForTests(ITodoDbTestContext db, IEnumerable<TodoItem> todoItems)
        {
            lock (_lock)
            {
                if (!db.HasItems<TodoItem>())
                {
                    db.Seed<TodoItem>(todoItems);
                }
            }
        }

        public static void ReinitializeDbForTests(ITodoDbTestContext db, IEnumerable<TodoItem> todoItems)
        {
            // While a lock is held, the thread that holds the lock can again acquire and release the lock,
            // so should be ok to call the InitializeDbForTests() with the same lock.
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement
            lock (_lock)
            {
                db.Clean<TodoItem>();
                InitializeDbForTests(db, todoItems);
            }
        }
    }
}
