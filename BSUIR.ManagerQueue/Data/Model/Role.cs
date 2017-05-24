using BSUIR.ManagerQueue.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Data.Model
{
    public class Role : IdentityRole<int, UserRole>, IEntity
    {
    }

    public static class RoleExtensions
    {
        public static UserType ToUserType(this IEnumerable<Role> userRoles)
        {
            if (userRoles == null || !userRoles.Any())
                return UserType.Employee;

            if (userRoles.Any(role => role.Name == RoleNames.Manager))
                return UserType.Manager;

            if (userRoles.Any(role => role.Name == RoleNames.Vice))
                return UserType.Vice;

            if (userRoles.Any(role => role.Name == RoleNames.Secretary))
                return UserType.Secretary;

            return UserType.Employee;
        }
    }
}
