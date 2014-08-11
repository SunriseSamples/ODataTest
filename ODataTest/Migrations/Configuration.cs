using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using ODataTest.Models;

namespace ODataTest.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<ProductsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ProductsContext context)
        {
            context.Products.AddOrUpdate(new Product[] {
                new Product() { ID = 1, Name = "Hat", Price = 15, Category = "Apparel" },
                new Product() { ID = 2, Name = "Socks", Price = 5, Category = "Apparel" },
                new Product() { ID = 3, Name = "Scarf", Price = 12, Category = "Apparel" },
                new Product() { ID = 4, Name = "Yo-yo", Price = 4.95M, Category = "Toys" },
                new Product() { ID = 5, Name = "Puzzle", Price = 8, Category = "Toys" },
            });
        }
    }
}
