using System.Data.Entity;

using Microsoft.AspNet.Identity.EntityFramework;

namespace BSUIR.ManagerQueue.Data
{
    using Infrastructure;
    using Model;

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
            context.Roles.Add(new Role() { Name = RoleNames.Administrator });
        }
    }
}
