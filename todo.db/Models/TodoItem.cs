using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo.db.Models
{
    // https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=data-annotations#table-schema
    [Table("TodoItem", Schema = "Todo")]
    public partial class TodoItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TodoItemId { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public DateTime Created { get; set; }
    }

    public partial class TodoItem
    {
        //To hide with feature flag
        //public string Note { get; set; }
        
        //NB!
        // Can NOT have a field here that don't match with the sql server schema!!
        // BUT we CAN have a new field in the SQL Server schema that is not included in the model

        // SO, we can NOT implement FF here. It needs to go into the DTO Mapper.
        // BUT we can add a partial class here with the NOTE field and name the class with the same name as the FF in order to keep track of changes.

        public string Hello()
        {
            return "hello";
        }

    }
}
