using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestAspService.Controllers
{
    using BSUIR.ManagerQueue.Data;
    using BSUIR.ManagerQueue.Data.Model;
    using BSUIR.ManagerQueue.Infrastructure;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using System.Data.Entity;
    using System.Threading.Tasks;

    [Authorize]
    public class QueueController : ApiController
    {
        private ApplicationDbContext dbContext;
        private ApplicationUserManager userManager;

        public QueueController()
        {
        }

        public QueueController(ApplicationDbContext dbContext, ApplicationUserManager userManager)
        {
            DbContext = dbContext;
            UserManager = userManager;
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

        // GET: api/Queue
        [Authorize(Roles = RoleNames.Manager + ", " + RoleNames.Vice)]
        public async Task<IEnumerable<QueueItem>> Get()
        {
            var userId = User.Identity.GetUserId<int>();
            return await dbContext.Queue.Where(queueItem => queueItem.ManagerId == userId).ToArrayAsync();
        }

        // GET: api/Queue/5
        [Authorize(Roles = RoleNames.Manager + ", " + RoleNames.Vice + ", " + RoleNames.Secretary + ", " + RoleNames.Administrator)]
        public async Task<IHttpActionResult> Get(int id)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!await UserHasAccessToQueue(userId, id))
                return Unauthorized();

            return Ok(await dbContext.Queue.Where(queueItem => queueItem.ManagerId == id).ToArrayAsync());
        }

        // POST: api/Queue
        public async Task<IHttpActionResult> Post([FromBody]IEnumerable<QueueItem> queueItems)
        {
            var queueIds = queueItems.Select(queueItem => queueItem.ManagerId).Distinct().ToArray();
            if (queueIds.Length != 1)
                return BadRequest();
            var queueId = queueIds[0];

            var originalQueue = await dbContext.Queue.Where(queueItem => queueItem.ManagerId == queueId).ToArrayAsync();
            
        }

        private async Task<bool> UserHasAccessToQueue(int userId, int queueId)
        {
            if (await UserManager.IsInRoleAsync(userId, RoleNames.Administrator))
                return true;

            if (await UserManager.IsInRoleAsync(userId, RoleNames.Manager) || await UserManager.IsInRoleAsync(userId, RoleNames.Vice))
                return userId == queueId;

            var user = await UserManager.FindByIdAsync(userId);
            return user.ManagedQueues.Any(queue => queue.Id == queueId);
        }
    }
}
