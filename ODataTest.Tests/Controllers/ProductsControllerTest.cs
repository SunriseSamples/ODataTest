using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataTest.Controllers;
using ODataTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ODataTest.Tests.Controllers
{
    public class ProductComparer : IEqualityComparer<Product>
    {
        public bool Equals(Product x, Product y)
        {
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }

            return x != null && y != null &&
                x.ID == y.ID &&
                x.Name == y.Name &&
                x.Price == y.Price &&
                x.Category == y.Category &&
                x.SupplierId == y.SupplierId;
        }

        public int GetHashCode(Product product)
        {
            var hashCode = product.ID.GetHashCode();
            hashCode ^= (product.Name == null ? 0 : product.Name.GetHashCode());
            hashCode ^= product.Price.GetHashCode();
            hashCode ^= (product.Category == null ? 0 : product.Category.GetHashCode());
            hashCode ^= (product.SupplierId == null ? 0 : product.SupplierId.GetHashCode());

            return hashCode;
        }
    }

    [TestClass]
    public class ProductsControllerTest
    {
        private ProductsContext db = new ProductsContext();
        private ProductsController controller = new ProductsController();
        private ProductComparer comparer = new ProductComparer();

        [TestInitialize]
        public void TestInitialize()
        {
            var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                @"..\..\..\ODataTest\App_Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        [TestMethod]
        public void ProductsController_Get()
        {
            var result = controller.Get();
            Assert.IsNotNull(result);
            var expected = db.Products.ToList();
            var actual = result.ToList();
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.IsTrue(expected.SequenceEqual(actual, comparer));
        }

        // 参考：http://www.asp.net/web-api/overview/testing-and-debugging/unit-testing-with-aspnet-web-api

        [TestMethod]
        public void ProductsController_GetByKey()
        {
            var actual = controller.Get(5);
            Assert.IsNotNull(actual);
            var expected = db.Products.Find(5);
            Assert.IsTrue(comparer.Equals(expected, actual));
        }

        //[TestMethod]
        //public void Post()
        //{
        //    var controller = new ProductsController();
        //    var product = new Product() { ID = 6, Name = "Pen", Price = 1.23M, Category = "Tools" };
        //    var result = controller.Post(product);
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(db.Products.Find(6), result);
        //}
    }
}
