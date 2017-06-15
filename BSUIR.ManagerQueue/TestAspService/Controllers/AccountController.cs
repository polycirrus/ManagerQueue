using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace TestAspService.Controllers
{
    using BSUIR.ManagerQueue.Data.Model;
    using BSUIR.ManagerQueue.Infrastructure.Models;
    using BSUIR.ManagerQueue.Infrastructure;
    using BSUIR.ManagerQueue.Data;
    using System.Data.Entity;

    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private ApplicationDbContext dbContext;
        private ApplicationUserManager userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationDbContext dbContext, ApplicationUserManager userManager)
        {
            DbContext = dbContext;
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }

        public ApplicationDbContext DbContext
        {
            get
            {
                return dbContext ?? Request.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                dbContext = value;
            }
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(User.Identity.AuthenticationType);
            return Ok();
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(Convert.ToInt32(User.Identity.GetUserId()), model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Authorize(Roles = RoleNames.Administrator)]
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityResult result;
            if (UserManager.HasPassword(model.UserId))
            {
                result = await UserManager.RemovePasswordAsync(model.UserId);

                if (!result.Succeeded)
                    return GetErrorResult(result);
            }

            result = await UserManager.AddPasswordAsync(model.UserId, model.NewPassword);

            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Employee()
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                Middlename = model.Middlename,
                LastName = model.LastName,
                PositionId = model.PositionId
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return GetErrorResult(result);

            var role = ApplicationUserManager.GetRoleFromUserType(model.UserType);
            if (!string.IsNullOrEmpty(role))
                await UserManager.AddToRoleAsync(user.Id, role);

            return Ok();
        }

        // GET api/Account
        public async Task<Employee> Get()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            user.Type = ApplicationUserManager.GetUserTypeFromRoles(userRoles);
            user.IsAdministrator = userRoles.Contains(RoleNames.Administrator);

            user.PasswordHash = null;

            return user;
        }

        // GET api/Account/QueueOwners
        [Route("QueueOwners")]
        public async Task<IEnumerable<Employee>> GetQueueOwners()
        {
            var roles = new[] { RoleNames.Manager, RoleNames.Vice };
            var queueOwnersIds = await dbContext.Roles.Where(role => roles.Contains(role.Name)).SelectMany(x => x.Users)
                .Select(userRole => userRole.UserId).ToListAsync();
            return await Task.WhenAll(queueOwnersIds.Select(async id => StripAccount(await UserManager.FindByIdAsync(id))));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
                userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private Employee StripAccount(Employee account)
        {
            account.PasswordHash = null;
            return account;
        }

        #endregion
    }
}
