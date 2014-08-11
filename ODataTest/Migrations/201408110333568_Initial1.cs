namespace ODataTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
            AddColumn("dbo.Products", "SupplierId", c => c.String(maxLength: 128));
            AddForeignKey("dbo.Products", "SupplierId", "dbo.Suppliers", "Key");
            CreateIndex("dbo.Products", "SupplierId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Products", new[] { "SupplierId" });
            DropForeignKey("dbo.Products", "SupplierId", "dbo.Suppliers");
            DropColumn("dbo.Products", "SupplierId");
            DropTable("dbo.Suppliers");
        }
    }
}
