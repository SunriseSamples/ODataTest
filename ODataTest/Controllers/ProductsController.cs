using Microsoft.Data.OData;
using Microsoft.Data.OData.Query;
using ODataTest.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Extensions;
using System.Web.Http.OData.Routing;
using System.Web.Http.Routing;

namespace ODataTest.Controllers
{
    public class ProductsController : ODataController
    {
        private ProductsContext db = new ProductsContext();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET odata/Products
        [EnableQuery]
        public IQueryable<Product> Get()
        {
            return db.Products;
        }

        // GET odata/Products(5)
        public Product Get([FromODataUri] int key)
        {
            var product = db.Products.SingleOrDefault(p => p.ID == key);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return product;
        }

        // POST odata/Products
        public async Task<IHttpActionResult> Post(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            await db.SaveChangesAsync();

            var location = new Uri(Url.Link("ODataRoute", new { id = product.ID }));
            return Created(location, product);
        }

        // PUT odata/Products(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Product product)
        {
            if (!ModelState.IsValid || key != product.ID)
            {
                return BadRequest(ModelState);
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Debug.WriteLine(ex);
                return NotFound();
            }

            return Ok(product);
        }

        // PATCH odata/Products(5)
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Product patchProduct)
        {
            if (!ModelState.IsValid || key != patchProduct.ID)
            {
                return BadRequest(ModelState);
            }

            var product = await db.Products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }

            if (!String.IsNullOrEmpty(patchProduct.Name))
            {
                product.Name = patchProduct.Name;
            }
            product.Price = patchProduct.Price;
            if (!String.IsNullOrEmpty(patchProduct.Category))
            {
                product.Category = patchProduct.Category;
            }
            if (!String.IsNullOrEmpty(patchProduct.SupplierId))
            {
                product.SupplierId = patchProduct.SupplierId;
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

            return Ok(product);
        }

        // DELETE odata/Products(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var product = await db.Products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Debug.WriteLine(ex);
                return NotFound();
            }

            return Ok(product);
        }

        // GET odata/Products(5)/Supplier
        public Supplier GetSupplier([FromODataUri] int key)
        {
            Product product = db.Products.FirstOrDefault(p => p.ID == key);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return product.Supplier;
        }

        // Helper method to extract the key from an OData link URI.
        private TKey GetKeyFromLinkUri<TKey>(Uri link)
        {
            TKey key = default(TKey);

            // Get the route that was used for this request.
            IHttpRoute route = Request.GetRouteData().Route;

            // Create an equivalent self-hosted route. 
            IHttpRoute newRoute = new HttpRoute(route.RouteTemplate,
                new HttpRouteValueDictionary(route.Defaults),
                new HttpRouteValueDictionary(route.Constraints),
                new HttpRouteValueDictionary(route.DataTokens), route.Handler);

            // Create a fake GET request for the link URI.
            var tmpRequest = new HttpRequestMessage(HttpMethod.Get, link);

            // Send this request through the routing process.
            var routeData = newRoute.GetRouteData(
                Request.GetConfiguration().VirtualPathRoot, tmpRequest);

            // If the GET request matches the route, use the path segments to find the key.
            if (routeData != null)
            {
                var path = tmpRequest.ODataProperties().Path;
                var segment = path.Segments.OfType<KeyValuePathSegment>().FirstOrDefault();
                if (segment != null)
                {
                    // Convert the segment into the key type.
                    key = (TKey)ODataUriUtils.ConvertFromUriLiteral(
                        segment.Value, ODataVersion.V3);
                }
            }
            return key;
        }

        // POST odata/Products(1)/$links/Supplier
        // PUT odata/Products(1)/$links/Supplier
        // BODY {"link":"http://localhost/odata/Suppliers('CTSO')"}
        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateLink([FromODataUri] int key, string navigationProperty, [FromBody] Uri link)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product product = await db.Products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }

            switch (navigationProperty)
            {
                case "Supplier":
                    string supplierKey = GetKeyFromLinkUri<string>(link);
                    Supplier supplier = await db.Suppliers.FindAsync(supplierKey);
                    if (supplier == null)
                    {
                        return NotFound();
                    }
                    product.Supplier = supplier;
                    await db.SaveChangesAsync();
                    return StatusCode(HttpStatusCode.NoContent);

                default:
                    return NotFound();
            }
        }

        // DELETE odata/Products(1)/$links/Supplier
        public async Task<IHttpActionResult> DeleteLink([FromODataUri] int key, string navigationProperty)
        {
            Product product = await db.Products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }

            switch (navigationProperty)
            {
                case "Supplier":
                    product.Supplier = null;
                    await db.SaveChangesAsync();
                    return StatusCode(HttpStatusCode.NoContent);

                default:
                    return NotFound();

            }
        }

        // DELETE odata/Customers(1)/$links/Orders(1)
        // void DeleteLink([FromODataUri] int key, string relatedKey, string navigationProperty);
    }
}
