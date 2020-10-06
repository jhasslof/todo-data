using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todo.db.api.Database.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
