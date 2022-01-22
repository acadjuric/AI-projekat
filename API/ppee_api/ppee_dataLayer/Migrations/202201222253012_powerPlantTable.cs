namespace ppee_dataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class powerPlantTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PowerPlants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaximumOutputPower = c.Int(nullable: false),
                        MinimumOutputPower = c.Int(nullable: false),
                        Name = c.String(),
                        Efficiency = c.Int(nullable: false),
                        SurfaceArea = c.Int(nullable: false),
                        BladesSweptAreaDiameter = c.Int(nullable: false),
                        NumberOfWindGenerators = c.Int(nullable: false),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PowerPlants");
        }
    }
}
