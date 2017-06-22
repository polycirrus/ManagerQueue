using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace BSUIR.ManagerQueue.Service.Controllers
{
    using BSUIR.ManagerQueue.Data.Model;
    using BSUIR.ManagerQueue.Infrastructure.Models;
    using BSUIR.ManagerQueue.Infrastructure;
    using BSUIR.ManagerQueue.Data;

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

        // GET api/Account/All
        [Route("All")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IEnumerable<Employee>> GetAll()
        {
            return (await UserManager.Users.ToArrayAsync()).Select(user => PrepareAccount(user));
        }

        // GET api/Account/QueueOwners
        [Route("QueueOwners")]
        public async Task<IEnumerable<Employee>> GetQueueOwners()
        {
            var roles = new[] { RoleNames.Manager, RoleNames.Vice };
            var queueOwnersIds = await DbContext.Roles.Where(role => roles.Contains(role.Name)).SelectMany(x => x.Users)
                .Select(userRole => userRole.UserId).ToListAsync();

            var owners = new List<Employee>();
            foreach (var ownerId in queueOwnersIds)
                owners.Add(PrepareAccount(await UserManager.FindByIdAsync(ownerId)));

            return owners;
        }

        // POST api/Account
        public async Task<IHttpActionResult> Post(SaveAccountInfoModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.Identity.GetUserId<int>();
            if (currentUserId != model.Id && !(await UserManager.IsInRoleAsync(currentUserId, RoleNames.Administrator)))
                return Unauthorized();

            var account = await UserManager.FindByIdAsync(model.Id);
            if (account == null)
                return BadRequest();

            account.FirstName = model.FirstName;
            account.Middlename = model.MiddleName;
            account.LastName = model.LastName;

            if (model.PositionId.HasValue)
            {
                var position = await DbContext.Positions.FindAsync(model.PositionId.Value);
                if (position == null)
                    return BadRequest();

                account.PositionId = model.PositionId.Value;
            }
            else if (!string.IsNullOrEmpty(model.JobTitle))
            {
                var position = new Position() { JobTitle = model.JobTitle };
                DbContext.Positions.Add(position);

                account.PositionId = 0;
                account.Position = position;
            }
            else
                return BadRequest();

            await DbContext.SaveChangesAsync();

            var role = ApplicationUserManager.GetRoleFromUserType(model.Type);
            var existingRoles = await UserManager.GetRolesAsync(account.Id);
            var existingRole = existingRoles?.FirstOrDefault(r => r != RoleNames.Administrator);
            if (role != existingRole)
            {
                if (existingRole != null)
                    await UserManager.RemoveFromRoleAsync(account.Id, existingRole);

                if (role != null)
                    await UserManager.AddToRoleAsync(account.Id, role);
            }

            return Ok(await UserManager.FindByIdAsync(account.Id));
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

        private Employee PrepareAccount(Employee account)
        {
            account.Type = ApplicationUserManager.GetUserTypeFromRoles(UserManager.GetRoles(account.Id));
            account.PasswordHash = null;
            return account;
        }

        #endregion
    }
}
