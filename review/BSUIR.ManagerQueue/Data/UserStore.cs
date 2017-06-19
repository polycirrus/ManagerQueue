using Microsoft.AspNet.Identity.EntityFramework;

namespace BSUIR.ManagerQueue.Data
{
    using Model;

    public class UserStore : UserStore<Employee, Role, int, UserLogin, UserRole, UserClaim>
    {
        public UserStore()
            : base(new ApplicationDbContext())
        {
        }

        public UserStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
