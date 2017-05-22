using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace BSUIR.ManagerQueue.Services
{
    using Data;
    using Data.Model;

    class CredentialsValidator : UserNamePasswordValidator
    {
        private UserStore store;
        private UserManager<Employee, int> manager;

        public CredentialsValidator()
        {
            store = new UserStore();
            manager = new UserManager<Employee, int>(store);
        }

        public override void Validate(string userName, string password)
        {
            if (userName == "c" && password == "d")
                return;
            throw new SecurityTokenException("Invalid user name and password combination.");

            //var user = manager.Find(userName, password);
            //if (user == null)
            //    throw new SecurityTokenException("Invalid user name and password combination.");
        }
    }
}
