using Microsoft.EntityFrameworkCore;
using ppee_dataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ppee_dataLayer
{
    public class PPEE_DataContext: DbContext
    {
        public PPEE_DataContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer("Server=(local)\\sqlexpress;Database=ProjekatAI;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }

        public DbSet<Load> Loads { get; set; }
    }
}
