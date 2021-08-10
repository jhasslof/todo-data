using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todo.db.Models
{
    [Table("SupportedFeature", Schema = "Internal")]
    public class SupportedFeature
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }

    }
}
