using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace TestAspService
{
    using BSUIR.ManagerQueue.Data.Model;
    using BSUIR.ManagerQueue.Data;
    using BSUIR.ManagerQueue.Infrastructure;
    using System.Threading.Tasks;

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<Employee, int>
    {
        public ApplicationUserManager(IUserStore<Employee, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore(context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<Employee, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<Employee, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public async Task<UserType> GetUserType(int userId)
        {
            var userRoles = await GetRolesAsync(userId);
            return GetUserTypeFromRoles(userRoles);
        }

        public static string GetRoleFromUserType(UserType userType)
        {
            switch (userType)
            {
                case UserType.Secretary:
                    return RoleNames.Secretary;
                case UserType.Vice:
                    return RoleNames.Vice;
                case UserType.Manager:
                    return RoleNames.Manager;
                default:
                    return null;
            }
        }

        public static UserType GetUserTypeFromRoles(ICollection<string> roleNames)
        {
            if (roleNames == null || !roleNames.Any())
                return UserType.Employee;

            if (roleNames.Any(role => role == RoleNames.Manager))
                return UserType.Manager;

            if (roleNames.Any(role => role == RoleNames.Vice))
                return UserType.Vice;

            if (roleNames.Any(role => role == RoleNames.Secretary))
                return UserType.Secretary;

            return UserType.Employee;
        }
    }
}
