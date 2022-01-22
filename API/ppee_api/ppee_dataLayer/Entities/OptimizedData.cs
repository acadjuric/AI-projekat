using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Entities
{
    public class OptimizedData
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public double Load { get; set; } = 0;
        public string Type { get; set; }
        public int Cost { get; set; } = 0;
        public int CO2 { get; set; } = 0;

        public OptimizationPerHour OptimizationPerHour { get; set; }


        public OptimizedData()
        {

        }
    }
}
