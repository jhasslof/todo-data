using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo.db.Models
{
    // https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=data-annotations#table-schema
    [Table("TodoItem", Schema = "Todo")]
    public partial class TodoItem
    {
        //NB!
        // Can NOT have a field here that don't match with the sql server schema!!
        // BUT we CAN have a new field in the SQL Server schema that is not included in the model
        // SO, we can NOT implement FF here. It needs to go into the DTO Mapper.

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TodoItemId { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public DateTime Created { get; set; }
    }
}
