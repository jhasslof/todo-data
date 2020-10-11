using System.Collections.Generic;

namespace todo.db.api.IntegrationTest.Context
{
    public interface ITodoDbTestContext : ITodoDbContext
    {
        IEnumerable<TModel> List<TModel>() where TModel : class;
        bool HasItems<TModel>() where TModel : class;
        void Seed<TModel>(IEnumerable<TModel> seedTodoItems) where TModel : class;
        void Clean<TModel>() where TModel : class;
    }
}
