using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BSUIR.ManagerQueue.Data.Model;

using Microsoft.AspNet.Identity.EntityFramework;

namespace BSUIR.ManagerQueue.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee, Role, int, UserLogin, UserRole, UserClaim>
    {
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<QueueItem> Queue { get; set; }

        public ApplicationDbContext()
            : base("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NewDb")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
        }

        public static ApplicationDbContext Create() => new ApplicationDbContext();

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.OwnQueueEntries)
                .WithRequired(q => q.Manager)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.ForeignQueueEntries)
                .WithRequired(q => q.Employee)
                .WillCascadeOnDelete(false);
        }
    }
}
