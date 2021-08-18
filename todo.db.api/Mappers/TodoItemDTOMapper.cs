using System.Collections.Generic;
using System.Linq;
using todo.db.api.Models;
using todo.db.Models;

namespace todo.db.api.Mappers
{
    public class TodoItemDTOMapper
    {
        IFeatureFlags _featureFlags;

        public TodoItemDTOMapper(IFeatureFlags featureFlags)
        {
            _featureFlags = featureFlags;
        }

        public TodoItemDTO Map(TodoItem todoItem)
        {
            if (todoItem == null)
                return null;

            var todoItemDto =  new TodoItemDTO
            {
                Id = todoItem.TodoItemId,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
            return todoItemDto;
        }

        public List<TodoItemDTO> Map(List<TodoItem> todoItems)
        {
            return todoItems.ConvertAll(new System.Converter<TodoItem, TodoItemDTO>(Map));
        }
    }
}
