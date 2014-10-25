namespace CloudApp.API.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tags", new[] { "Name" });
            DropTable("dbo.Tags");
        }
    }
}
