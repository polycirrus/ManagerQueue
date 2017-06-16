using System.Data.Entity;
using System.Linq;
using BSUIR.ManagerQueue.Data.Model;
using BSUIR.ManagerQueue.Infrastructure;
using MoreLinq;

namespace BSUIR.ManagerQueue.Data
{
    public class DemoDataInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        private static readonly string PasswordHash = @"AHEbLJ5gfvg6SRgtqt6K8QoKSRZ8XquiV5CMNxv6f7AOdmZIYEBIHX+ktnUnGRHaNA=="; //abcd1234
        private static readonly string SecurityStamp = @"4a5cb7b6-7829-4f45-ab55-788b6777a0ab";

        protected override void Seed(ApplicationDbContext context)
        {
            var ceo = CreateUser("Gregory", "Parker", "Chief Executive Officer", "ceo");
            var salesHead = CreateUser("Margaret", "Long", "Head of Sales", "salesHead");
            var salesVice = CreateUser("Kelly", "Cox", "Vice Head of Sales", "salesVice");
            var rdHead = CreateUser("Larry", "Davis", "Head of Research and Development", "rdHead");
            var rdVice = CreateUser("Diane", "Ward", "Vice Head of Research and Development", "rdVice");
            var itHead = CreateUser("Daniel", "Peterson", "Head of IT", "itHead");
            var itVice = CreateUser("Kathryn", "Watson", "Vice Head of IT", "itVice");

            var ceoSecretary = CreateUser("Melissa", "Wright", "Personal Assistant", "ceoSecretary");
            var salesSecretary = CreateUser("Beverly", "Brooks", "Secretary, Sales", "salesSecretary");
            var rdSecretary = CreateUser("Fred", "Miller", "Secretary, Research and Development", "rdSecretary");

            var rdPosition = new Position() {JobTitle = "Researcher"};
            var rd1 = CreateUser("Gary", "Evans", rdPosition, "rd1");
            var rd2 = CreateUser("Arthur", "Baker", rdPosition, "rd2");
            var rd3 = CreateUser("Anthony", "Taylor", rdPosition, "rd3");

            var salesPosition = new Position() {JobTitle = "Sales Manager"};
            var sales1 = CreateUser("Catherine", "Simmons", salesPosition, "sales1");
            var sales2 = CreateUser("Victor", "White", salesPosition, "sales2");
            var sales3 = CreateUser("Lillian", "Lee", salesPosition, "sales3");

            var clerkPosition = new Position() {JobTitle = "Clerk"};
            var clerk1 = CreateUser("Bruce", "Butler", clerkPosition, "clerk1");
            var clerk2 = CreateUser("Ronald", "Taylor", clerkPosition, "clerk2");
            var clerk3 = CreateUser("Susan", "Perez", clerkPosition, "clerk3");
            var clerk4 = CreateUser("Earl", "Martinez", clerkPosition, "clerk4");
            var clerk5 = CreateUser("Thomas", "Brooks", clerkPosition, "clerk5");
            var clerks = new[] {clerk1, clerk2, clerk3, clerk4, clerk5};

            var itPosition = new Position() {JobTitle = "IT Engineer"};
            var it1 = CreateUser("Gregory", "Hill", itPosition, "it1");
            var it2 = CreateUser("Amanda", "Anderson", itPosition, "it2");
            var it3 = CreateUser("Lisa", "Thomas", itPosition, "it3");

            foreach (var employee in new[] {sales1, sales2, sales3}.Concat(clerks).RandomSubset(8))
                context.Queue.Add(new QueueItem() {Manager = salesHead, Employee = employee});

            foreach (var employee in new[] {rd1, rd2, rd3}.Concat(clerks).RandomSubset(8))
                context.Queue.Add(new QueueItem() { Manager = rdHead, Employee = employee });

            foreach (var employee in new[] {it1, it2, it3}.Concat(clerks).RandomSubset(8))
                context.Queue.Add(new QueueItem() { Manager = itHead, Employee = employee });

            var employees = new[]
            {
                ceo, ceoSecretary,
                salesHead, salesVice, salesSecretary, sales1, sales2, sales3,
                rdHead, rdVice, rdSecretary, rd1, rd2, rd3,
                itHead, itVice, it1, it2, it3,
                clerk1, clerk2, clerk3, clerk4, clerk5
            };
            foreach (var employee in employees)
            {
                context.Users.Add(employee);
            }

            var secretaryRole = new Role() {Name = RoleNames.Secretary};
            var viceRole = new Role() {Name = RoleNames.Vice};
            var managerRole = new Role() {Name = RoleNames.Manager};
            var administratorRole = new Role() {Name = RoleNames.Administrator};

            AddUserToRole(ceoSecretary, secretaryRole);
            AddUserToRole(salesSecretary, secretaryRole);
            AddUserToRole(rdSecretary, secretaryRole);

            AddUserToRole(ceo, managerRole);
            AddUserToRole(salesHead, managerRole);
            AddUserToRole(rdHead, managerRole);
            AddUserToRole(itHead, managerRole);

            AddUserToRole(salesVice, viceRole);
            AddUserToRole(rdVice, viceRole);
            AddUserToRole(itVice, viceRole);

            AddUserToRole(itHead, administratorRole);
            AddUserToRole(itVice, administratorRole);
            AddUserToRole(it1, administratorRole);
            AddUserToRole(it2, administratorRole);
            AddUserToRole(it3, administratorRole);

            context.Roles.Add(secretaryRole);
            context.Roles.Add(viceRole);
            context.Roles.Add(managerRole);
            context.Roles.Add(administratorRole);
        }

        private static Employee CreateUser(string firstName, string lastName, string jobTitle, string userName)
        {
            return CreateUser(firstName, lastName, new Position() {JobTitle = jobTitle}, userName);
        }

        private static Employee CreateUser(string firstName, string lastName, Position position, string userName)
        {
            return new Employee()
            {
                FirstName = firstName,
                LastName = lastName,
                Position = position,
                UserName = userName,
                PasswordHash = PasswordHash,
                SecurityStamp = SecurityStamp
            };
        }

        private static void AddUserToRole(Employee user, Role role)
        {
            var userRoleItem = new UserRole();
            user.Roles.Add(userRoleItem);
            role.Users.Add(userRoleItem);
        }
    }
}