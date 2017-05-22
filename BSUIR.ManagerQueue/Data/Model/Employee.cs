using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace BSUIR.ManagerQueue.Data.Model
{
    public class Employee : IdentityUser<int, UserLogin, UserRole, UserClaim>, IEntity
    {
        [Required]
        public string FirstName { get; set; }
        public string Middlename { get; set; }
        [Required]
        public string LastName { get; set; }

        public int PositionId { get; set; }
        //[Required]
        public virtual Position Position { get; set; }

        /// <summary>
        /// Entries in this employee's own queue.
        /// </summary>
        public virtual ICollection<QueueItem> OwnQueueEntries { get; set; }

        /// <summary>
        /// This employee's entries to other employee's queues.
        /// </summary>
        public virtual ICollection<QueueItem> ForeignQueueEntries { get; set; }
    }
}
