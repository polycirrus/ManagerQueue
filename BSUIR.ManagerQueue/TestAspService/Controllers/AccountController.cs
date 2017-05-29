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

    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
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

            var role = GetRoleFromUserType(model.UserType);
            if (!string.IsNullOrEmpty(role))
                await UserManager.AddToRoleAsync(user.Id, role);

            return Ok();
        }

        // GET api/Account
        public async Task<Employee> Get()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            user.Type = GetUserTypeFromRoles(userRoles);
            user.IsAdministrator = userRoles.Contains(RoleNames.Administrator);

            user.PasswordHash = null;

            return user;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
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

        private static string GetRoleFromUserType(UserType userType)
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

        private static UserType GetUserTypeFromRoles(ICollection<string> roleNames)
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

        #endregion
    }
}
