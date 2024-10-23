using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ECP_WEBAPI.Models;

namespace ECP_WEBAPI.Controllers
{
    public class PhienLamViecController : ApiController
    {
        private ECP_Model db = new ECP_Model();

        // GET: api/PhienLamViec
        public IQueryable<tblPhienLamViec> GettblPhienLamViecs()
        {
            return db.tblPhienLamViecs;
        }

        // GET: api/PhienLamViec/5
        [ResponseType(typeof(tblPhienLamViec))]
        public async Task<IHttpActionResult> GettblPhienLamViec(int id)
        {
            tblPhienLamViec tblPhienLamViec = await db.tblPhienLamViecs.FindAsync(id);
            if (tblPhienLamViec == null)
            {
                return NotFound();
            }

            return Ok(tblPhienLamViec);
        }

        // PUT: api/PhienLamViec/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PuttblPhienLamViec(int id, tblPhienLamViec tblPhienLamViec)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tblPhienLamViec.Id)
            {
                return BadRequest();
            }

            db.Entry(tblPhienLamViec).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblPhienLamViecExists(id))
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

        // POST: api/PhienLamViec
        [ResponseType(typeof(tblPhienLamViec))]
        public async Task<IHttpActionResult> PosttblPhienLamViec(tblPhienLamViec tblPhienLamViec)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tblPhienLamViecs.Add(tblPhienLamViec);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = tblPhienLamViec.Id }, tblPhienLamViec);
        }

        // DELETE: api/PhienLamViec/5
        [ResponseType(typeof(tblPhienLamViec))]
        public async Task<IHttpActionResult> DeletetblPhienLamViec(int id)
        {
            tblPhienLamViec tblPhienLamViec = await db.tblPhienLamViecs.FindAsync(id);
            if (tblPhienLamViec == null)
            {
                return NotFound();
            }

            db.tblPhienLamViecs.Remove(tblPhienLamViec);
            await db.SaveChangesAsync();

            return Ok(tblPhienLamViec);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblPhienLamViecExists(int id)
        {
            return db.tblPhienLamViecs.Count(e => e.Id == id) > 0;
        }
    }
}