using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo.db.Models
{
    // https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=data-annotations#table-schema
    [Table("TodoItem", Schema = "Todo")]
    public class TodoItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TodoItemId { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public DateTime Created { get; set; }

    }
}
