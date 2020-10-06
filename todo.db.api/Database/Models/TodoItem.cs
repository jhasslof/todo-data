using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace todo.db.api.Database.Models
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
