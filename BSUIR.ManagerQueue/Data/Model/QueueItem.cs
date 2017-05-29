using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Data.Model
{
    [Table("Queue")]
    public class QueueItem : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ManagerId { get; set; }
        public virtual Employee Manager { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public int Order { get; set; }
    }
}
