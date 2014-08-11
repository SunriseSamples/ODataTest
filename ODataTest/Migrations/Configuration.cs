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
            // 执行数据库升级操作，请在“程序包管理器控制台”下执行下面的命令：
            // Add-Migration Initial
            // Update-Database

            context.Products.AddOrUpdate(new Product[] {
                new Product() { ID = 1, Name = "Hat", Price = 15, Category = "Apparel" },
                new Product() { ID = 2, Name = "Socks", Price = 5, Category = "Apparel" },
                new Product() { ID = 3, Name = "Scarf", Price = 12, Category = "Apparel" },
                new Product() { ID = 4, Name = "Yo-yo", Price = 4.95M, Category = "Toys" },
                new Product() { ID = 5, Name = "Puzzle", Price = 8, Category = "Toys", SupplierId = "CTSO" },
            });

            context.Suppliers.AddOrUpdate(new Supplier[] {
                new Supplier() { Key = "CTSO", Name = "CTSO Name" },
                new Supplier() { Key = "ET", Name = "ET Name" },
                new Supplier() { Key = "Mars", Name = "Mars Name" },
                new Supplier() { Key = "Sun", Name = "Sun Name" },
                new Supplier() { Key = "Moon", Name = "Moon Name" },
            });
        }
    }
}
