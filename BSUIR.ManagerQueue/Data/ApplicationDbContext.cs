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
using BSUIR.ManagerQueue.Infrastructure;

namespace BSUIR.ManagerQueue.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee, Role, int, UserLogin, UserRole, UserClaim>
    {
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<QueueItem> Queue { get; set; }

        public ApplicationDbContext()
            : base()
        {
            Database.SetInitializer(new Initializer());
        }

        public static ApplicationDbContext Create() => new ApplicationDbContext();

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaim");

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.OwnQueueEntries)
                .WithRequired(q => q.Manager)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.ForeignQueueEntries)
                .WithRequired(q => q.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.ManagedQueues)
                .WithMany(e => e.QueueSecretaries)
                .Map(config =>
                {
                    config.MapLeftKey("SecretaryId");
                    config.MapRightKey("ManagerId");
                    config.ToTable("QueueSecretary");
                });
        }
    }

    public class Initializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.Roles.Add(new Role() { Name = RoleNames.Secretary });
            context.Roles.Add(new Role() { Name = RoleNames.Vice });
            context.Roles.Add(new Role() { Name = RoleNames.Manager });
        }
    }
}
