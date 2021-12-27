namespace ppee_dataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forecast_values_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForecastValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateAndTime = c.String(),
                        Load = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ForecastValues");
        }
    }
}
