using System.Collections.Generic;
using todo.db.api.Database.Models;
using todo.db.api.Models;

namespace todo.db.api.Mappers
{
    public class TodoItemDTOMapper
    {
        public static TodoItemDTO Map(TodoItem todoItem)
        {
            if (todoItem == null)
                return null;

            return new TodoItemDTO
            {
                Id = todoItem.TodoItemId,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
         }

        public static List<TodoItemDTO> Map(List<TodoItem> todoItems)
        {
            return todoItems.ConvertAll(new System.Converter<TodoItem, TodoItemDTO>(Map));
        }

    }
}
