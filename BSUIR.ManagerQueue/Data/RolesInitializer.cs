using System.Data.Entity;
using BSUIR.ManagerQueue.Data.Model;
using BSUIR.ManagerQueue.Infrastructure;

namespace BSUIR.ManagerQueue.Data
{
    public class RolesInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
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