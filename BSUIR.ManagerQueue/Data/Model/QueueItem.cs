using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Data.Model
{
    public class QueueItem : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual Employee Manager { get; set; }
        [Required]
        public virtual Employee Employee { get; set; }
        public int Order { get; set; }
    }
}
