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

        public DbSet<Load> Loads { get; set; }
    }
}
