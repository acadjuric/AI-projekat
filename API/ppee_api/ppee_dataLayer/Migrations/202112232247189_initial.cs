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
                        DayOfWeek = c.Single(nullable: false),
                        Hour = c.Single(nullable: false),
                        Month = c.Single(nullable: false),
                        AirTemperature = c.Single(nullable: false),
                        AtmosphericPressure = c.Single(nullable: false),
                        PressureTendency = c.Single(nullable: false),
                        RelativeHumidity = c.Single(nullable: false),
                        MeanWindSpeed = c.Single(nullable: false),
                        MaxGustValue = c.Single(nullable: false),
                        TotalCloudCover = c.Single(nullable: false),
                        Visibility = c.Single(nullable: false),
                        DewPointTemperature = c.Single(nullable: false),
                        MWh = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WeatherAndLoads");
        }
    }
}
