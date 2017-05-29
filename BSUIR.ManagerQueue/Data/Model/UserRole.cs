using System.Collections.Generic;

using Microsoft.AspNet.Identity.EntityFramework;

namespace BSUIR.ManagerQueue.Data.Model
{
    public class UserRole : IdentityUserRole<int>
    {
        public virtual Role Role { get; set; }
    }
}
