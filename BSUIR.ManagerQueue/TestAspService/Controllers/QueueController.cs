using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace TestAspService.Controllers
{
    using BSUIR.ManagerQueue.Data;
    using BSUIR.ManagerQueue.Data.Model;
    using BSUIR.ManagerQueue.Infrastructure;
    using System.Web.Http.Description;

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
        [ResponseType(typeof(IEnumerable<QueueItem>))]
        public async Task<IHttpActionResult> Get(int id)
        {
            var userId = User.Identity.GetUserId<int>();
            if (!await UserHasAccessToQueue(userId, id))
                return Unauthorized();

            return Ok(await dbContext.Queue.Where(queueItem => queueItem.ManagerId == id).ToArrayAsync());
        }

        // GET: api/Queue/GetEntries
        [HttpGet]
        [ResponseType(typeof(IEnumerable<QueueItem>))]
        public async Task<IHttpActionResult> GetEntries()
        {
            var userId = User.Identity.GetUserId<int>();
            return await GetEntries(userId);
        }

        // GET: api/Queue/GetEntries/5
        [HttpGet]
        [Authorize(Roles = RoleNames.Administrator)]
        [ResponseType(typeof(IEnumerable<QueueItem>))]
        public async Task<IHttpActionResult> GetEntries(int userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest();

            var entries = user.ForeignQueueEntries;
            return Ok(entries);
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostEntry(int queueId, int entrantId)
        {
            var currentUserId = User.Identity.GetUserId<int>();
            if (entrantId != currentUserId && !await UserManager.IsInRoleAsync(currentUserId, RoleNames.Administrator))
                return Unauthorized();

            var manager = await UserManager.FindByIdAsync(queueId);
            if (manager == null)
                return BadRequest();

            var queue = manager.OwnQueueEntries;
            if (queue.Any(queueItem => queueItem.EmployeeId == entrantId))
                return BadRequest();

            var newEntryOrder = queue.Max(queueItem => queueItem.Order) + 1;
            var newEntry = new QueueItem()
            {
                ManagerId = queueId,
                EmployeeId = entrantId,
                Order = newEntryOrder
            };

            dbContext.Queue.Add(newEntry);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // PUT: api/Queue/5
        public async Task<IHttpActionResult> Put(int queueId, [FromBody]IEnumerable<QueueItem> queueItems)
        {
            var queueIds = queueItems.Select(queueItem => queueItem.ManagerId).Distinct().ToArray();
            if (queueIds.Length != 1 || queueIds[0] != queueId)
                return BadRequest();

            var userId = User.Identity.GetUserId<int>();
            if (!await UserHasAccessToQueue(userId, queueId))
                return Unauthorized();

            var originalQueue = await dbContext.Queue.Where(queueItem => queueItem.ManagerId == queueId).ToArrayAsync();
            if (originalQueue.Length != queueItems.Count())
                return BadRequest();

            throw new NotImplementedException();
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
