using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ODataTest.Models;
using System.Web.Http.OData;

namespace ODataTest.Controllers
{
    public class ProductsController : ODataController
    {
        private ProductsContext db = new ProductsContext();

        // GET odata/Products
        [Queryable]
        public IQueryable<Product> Get()
        {
            return db.Products;
        }

        // GET odata/Products/5
        public Product Get(int id)
        {
            var product = db.Products.SingleOrDefault(p => p.ID == id);
            if (product == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return product;
        }

        // PUT odata/Products/5
        public HttpResponseMessage PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != product.ID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST odata/Products
        public HttpResponseMessage PostProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, product);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = product.ID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE odata/Products/5
        public HttpResponseMessage DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Products.Remove(product);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, product);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}