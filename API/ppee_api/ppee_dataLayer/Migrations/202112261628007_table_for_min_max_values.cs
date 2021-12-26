namespace ppee_dataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class table_for_min_max_values : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinMaxValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinAirTemperature = c.Single(nullable: false),
                        MaxAirTemperature = c.Single(nullable: false),
                        MinHumidity = c.Single(nullable: false),
                        MaxHumidity = c.Single(nullable: false),
                        MinAtmosphericPressure = c.Single(nullable: false),
                        MaxAtmosphericPressure = c.Single(nullable: false),
                        MinPressureTendency = c.Single(nullable: false),
                        MaxPressureTendency = c.Single(nullable: false),
                        MinMeanWindSpeed = c.Single(nullable: false),
                        MaxMeanWindSpeed = c.Single(nullable: false),
                        MinMaxGustValue = c.Single(nullable: false),
                        MaxMaxGustValue = c.Single(nullable: false),
                        MinTotalCloudCover = c.Single(nullable: false),
                        MaxTotalCloudCover = c.Single(nullable: false),
                        MinVisibility = c.Single(nullable: false),
                        MaxVisibility = c.Single(nullable: false),
                        MinDewPointTemperature = c.Single(nullable: false),
                        MaxDewPointTemperature = c.Single(nullable: false),
                        MinMWh = c.Single(nullable: false),
                        MaxMWh = c.Single(nullable: false),
                        MinHour = c.Single(nullable: false),
                        MaxHour = c.Single(nullable: false),
                        MinMonth = c.Single(nullable: false),
                        MaxMonth = c.Single(nullable: false),
                        MinDayOfWeek = c.Single(nullable: false),
                        MaxDayOfWeek = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MinMaxValues");
        }
    }
}
