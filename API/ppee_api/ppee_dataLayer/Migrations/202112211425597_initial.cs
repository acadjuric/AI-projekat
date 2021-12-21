namespace ppee_dataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeatherAndLoads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.String(),
                        AirTemperature = c.Double(nullable: false),
                        AtmosphericPressure = c.Double(nullable: false),
                        PressureTendency = c.Double(nullable: false),
                        RelativeHumidity = c.Double(nullable: false),
                        MeanWindSpeed = c.Double(nullable: false),
                        MaxGustValue = c.Double(nullable: false),
                        TotalCloudCover = c.Double(nullable: false),
                        Visibility = c.Double(nullable: false),
                        DewPointTemperature = c.Double(nullable: false),
                        MWh = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WeatherAndLoads");
        }
    }
}
