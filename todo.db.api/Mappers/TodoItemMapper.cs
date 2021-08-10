using todo.db.api.Models;
using todo.db.Models;

namespace todo.db.api.Mappers
{
    public class TodoItemMapper
    {
        IFeatureFlags _featureFlags;

        public TodoItemMapper(IFeatureFlags featureFlags)
        {
            _featureFlags = featureFlags;
        }

        public TodoItem Map(TodoItemDTO todoItemDto) =>
            new TodoItem
            {
                TodoItemId = todoItemDto.Id,
                Name = todoItemDto.Name,
                IsComplete = todoItemDto.IsComplete
            };

        public void Fill(TodoItem todoItem, TodoItemDTO todoItemDto)
        {
            todoItem.Name = todoItemDto.Name;
            todoItem.IsComplete = todoItemDto.IsComplete;
        }
    }
}
