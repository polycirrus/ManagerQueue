using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using BSUIR.ManagerQueue.Infrastructure;

namespace BSUIR.ManagerQueue.Data.Model
{
    [Table("Employee")]
    public class Employee : IdentityUser<int, UserLogin, UserRole, UserClaim>, IEntity
    {
        [Required]
        public string FirstName { get; set; }
        public string Middlename { get; set; }
        [Required]
        public string LastName { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string FullName => !string.IsNullOrEmpty(Middlename) ? $"{FirstName} {Middlename} {LastName}" : Name;

        [NotMapped]
        [JsonIgnore]
        public string Name => $"{FirstName} {LastName}";

        [NotMapped]
        public UserType Type { get; set; }

        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        /// <summary>
        /// Entries in this employee's own queue.
        /// </summary>
        public virtual ICollection<QueueItem> OwnQueueEntries { get; set; }

        /// <summary>
        /// This employee's entries to other employees' queues.
        /// </summary>
        public virtual ICollection<QueueItem> ForeignQueueEntries { get; set; }

        /// <summary>
        /// Queues, managed by this employee (only applicable to Secretaries).
        /// </summary>
        public virtual ICollection<Employee> ManagedQueues { get; set; }

        /// <summary>
        /// Secretaries who can manage this employee's own queue.
        /// </summary>
        public virtual ICollection<Employee> QueueSecretaries { get; set; }
    }
}
