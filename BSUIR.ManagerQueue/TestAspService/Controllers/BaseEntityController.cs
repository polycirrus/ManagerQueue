using BSUIR.ManagerQueue.Data;
using BSUIR.ManagerQueue.Data.Model;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace TestAspService.Controllers
{
    public abstract class BaseEntityController<TEntity> : ApiController
        where TEntity : class, IEntity
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Entity
        public virtual IQueryable<TEntity> Get()
        {
            return db.Set<TEntity>();
        }

        // GET: api/Entity/5
        //[ResponseType(typeof(TEntity))]
        public virtual async Task<IHttpActionResult> Get(int id)
        {
            var item = await db.Set<TEntity>().FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // PUT: api/Entity/5
        [ResponseType(typeof(void))]
        public virtual async Task<IHttpActionResult> Put(int id, TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != entity.Id)
            {
                return BadRequest();
            }

            db.Entry(entity).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Entity
        //[ResponseType(typeof(TEntity))]
        public virtual async Task<IHttpActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Set<TEntity>().Add(entity);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = entity.Id }, entity);
        }

        // DELETE: api/Entity/5
        //[ResponseType(typeof(TEntity))]
        public virtual async Task<IHttpActionResult> Delete(int id)
        {
            TEntity entity = await db.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            db.Set<TEntity>().Remove(entity);
            await db.SaveChangesAsync();

            return Ok(entity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EntityExists(int id)
        {
            return db.Set<TEntity>().Count(e => e.Id == id) > 0;
        }
    }
}
