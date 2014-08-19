using ODataTest.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;

namespace ODataTest.Controllers
{
    public class SuppliersController : ODataController
    {
        private ProductsContext db = new ProductsContext();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET odata/Suppliers
        [EnableQuery]
        public IQueryable<Supplier> Get()
        {
            return db.Suppliers;
        }

        // GET odata/Suppliers('CTSO')
        public Supplier Get([FromODataUri] string key)
        {
            var supplier = db.Suppliers.Find(key);
            if (supplier == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return supplier;
        }

        // POST odata/Suppliers
        public async Task<IHttpActionResult> Post(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Suppliers.Add(supplier);
            await db.SaveChangesAsync();

            var location = new Uri(Url.Link("ODataRoute", new { key = supplier.Key }));
            return Created(location, supplier);
        }

        // PUT odata/Suppliers('CTSO')
        public async Task<IHttpActionResult> Put([FromODataUri] string key, Supplier supplier)
        {
            if (!ModelState.IsValid || key != supplier.Key)
            {
                return BadRequest(ModelState);
            }

            db.Entry(supplier).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Debug.WriteLine(ex);
                return NotFound();
            }

            return Ok(supplier);
        }

        // PATCH odata/Suppliers('CTSO')
        public async Task<IHttpActionResult> Patch([FromODataUri] string key, Supplier patchSupplier)
        {
            if (!ModelState.IsValid || key != patchSupplier.Key)
            {
                return BadRequest(ModelState);
            }

            var supplier = await db.Suppliers.FindAsync(key);
            if (supplier == null)
            {
                return NotFound();
            }

            if (!String.IsNullOrEmpty(patchSupplier.Name))
            {
                supplier.Name = patchSupplier.Name;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Debug.WriteLine(ex);
                return NotFound();
            }

            return Ok(supplier);
        }

        // DELETE odata/Suppliers('CTSO')
        public async Task<IHttpActionResult> Delete([FromODataUri] string key)
        {
            var supplier = await db.Suppliers.FindAsync(key);
            if (supplier == null)
            {
                return NotFound();
            }

            db.Suppliers.Remove(supplier);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Debug.WriteLine(ex);
                return NotFound();
            }

            return Ok(supplier);
        }
    }
}
