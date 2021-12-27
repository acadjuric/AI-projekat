using ppee_dataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer
{
    using SqlProviderServices = System.Data.Entity.SqlServer.SqlProviderServices;

    public class PPEE_DataContext:DbContext
    {
        

        public PPEE_DataContext():base("name=ProjekatAI")
        {

        }

        public DbSet<WeatherAndLoad> WeatherAndLoads { get; set; }
        public DbSet<MinMaxValues> MinMaxValues { get; set; }
        public DbSet<ForecastValues> ForecastValues { get; set; }


    }
}
