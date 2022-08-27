namespace Task.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CustomerProducts", "CustomerId", "dbo.Customers");
            DropIndex("dbo.CustomerProducts", new[] { "ProductId" });
            DropIndex("dbo.CustomerProducts", new[] { "CustomerId" });
            DropTable("dbo.Customers");
            DropTable("dbo.CustomerProducts");
        }
    }
}
