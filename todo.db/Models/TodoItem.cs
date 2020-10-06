using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo.db.Models
{
    public class TodoItem
    {
        public long TodoItemId { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; }

    }
}
