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
    using BSUIR.ManagerQueue.Infrastructure.Models;
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

        // PUT: api/Queue/5
        public async Task<IHttpActionResult> Put(int queueId, [FromBody]IEnumerable<QueueItem> queueItems)
        {
            var queueIds = queueItems.Select(queueItem => queueItem.ManagerId).Distinct().ToArray();
            if (queueIds.Length != 1 || queueIds[0] != queueId)
                return BadRequest();

            if (!Enumerable.SequenceEqual(queueItems.OrderBy(queueItem => queueItem.Order).Select(queueItem => queueItem.Order), Enumerable.Range(0, queueItems.Count() - 1)))
                return BadRequest();

            var userId = User.Identity.GetUserId<int>();
            if (!await UserHasAccessToQueue(userId, queueId))
                return Unauthorized();

            var originalQueue = await dbContext.Queue.Where(queueItem => queueItem.ManagerId == queueId).OrderBy(queueItem => queueItem.EmployeeId).ToArrayAsync();
            if (originalQueue.Length != queueItems.Count())
                return BadRequest();

            queueItems = queueItems.OrderBy(queueItem => queueItem.Employee);
            if (!Enumerable.SequenceEqual(queueItems.Select(queueItem => queueItem.EmployeeId), originalQueue.Select(queueItem => queueItem.EmployeeId)))
                return BadRequest();

            originalQueue.Zip(queueItems.Select(queueItem => queueItem.Order), (original, order) => original.Order = order);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Queue/Entry
        [HttpPost]
        [Route("Entry")]
        public async Task<IHttpActionResult> PostEntry(AddQueueEntryModel queueItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (queueItem.EntrantId == queueItem.QueueId)
                return BadRequest();

            var userId = User.Identity.GetUserId<int>();
            if (queueItem.EntrantId != userId && !await UserHasAccessToQueue(userId, queueItem.QueueId))
                return Unauthorized();

            var queueManager = await UserManager.FindByIdAsync(queueItem.QueueId);
            if (queueManager == null)
                return BadRequest();

            var entrant = await UserManager.FindByIdAsync(queueItem.EntrantId);
            if (entrant == null)
                return BadRequest();

            return Ok(await AddQueueEntry(queueManager, entrant));
        }

        private async Task<QueueItem> AddQueueEntry(Employee queueManager, Employee entrant)
        {
            var queue = queueManager.OwnQueueEntries.OrderBy(item => item.Order).ToArray();

            var queueItem = new QueueItem()
            {
                ManagerId = queueManager.Id,
                EmployeeId = entrant.Id
            };

            var entrantType = await UserManager.GetUserType(entrant.Id);
            if (entrantType == UserType.Employee || entrantType == UserType.Secretary || queue.Length == 0)
            {
                queueItem.Order = queue.Length;
            }
            else
            {
                var sliceSize = entrantType == UserType.Manager ? 2 : 3;
                if (queue.Length <= sliceSize)
                    queueItem.Order = queue.Length;
                else
                {
                    var currentSliceCount = 0;
                    foreach (var item in queue)
                    {
                        if (await UserManager.GetUserType(item.EmployeeId) == UserType.Employee)
                        {
                            if (++currentSliceCount >= sliceSize)
                            {
                                queueItem.Order = item.Order;
                            }
                        }
                        else
                            currentSliceCount = 0;
                    }
                }
            }

            for (int i = queueItem.Order + 1; i < queue.Length; i++)
                queue[i].Order += 1;

            dbContext.Queue.Add(queueItem);
            await dbContext.SaveChangesAsync();
            return queueItem;
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
