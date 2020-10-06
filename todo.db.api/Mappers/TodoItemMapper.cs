using todo.db.api.Database.Models;
using todo.db.api.Models;

namespace todo.db.api.Mappers
{
    public class TodoItemMapper
    {
        public static TodoItem Map(TodoItemDTO todoItemDto) =>
            new TodoItem
            {
                TodoItemId = todoItemDto.Id,
                Name = todoItemDto.Name,
                IsComplete = todoItemDto.IsComplete
            };

        public static void Fill(TodoItem todoItem, TodoItemDTO todoItemDto)
        {
            todoItem.Name = todoItemDto.Name;
            todoItem.IsComplete = todoItemDto.IsComplete;
        }
    }
}
