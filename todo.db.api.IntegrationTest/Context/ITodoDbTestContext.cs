using System.Collections.Generic;

namespace todo.db.api.IntegrationTest.Context
{
    public interface ITodoDbTestContext : ITodoDbContext
    {
        void Seed<TModel>(IEnumerable<TModel> seedTodoItems) where TModel : class;
        void Clean<TModel>() where TModel : class;
    }
}
