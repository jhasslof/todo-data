using System.Collections.Generic;
using System.Threading.Tasks;
using todo.db.Models;

namespace todo.db
{
    public interface ITodoDbContext
    {
        public Task<List<TodoItem>> ListTodoItems();
        public Task<TodoItem> Get(long id);
        public Task Update(TodoItem editItem);
        public Task Delete(long id);
        public Task<TodoItem> Create(TodoItem newItem);
    }
}
