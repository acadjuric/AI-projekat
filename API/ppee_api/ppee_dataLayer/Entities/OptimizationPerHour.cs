using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Entities
{
    public class OptimizationPerHour
    {

        //[Key]
        public DateTime DateTimeOfOptimization { get; set; }
        public int Load { get; set; }
        public ICollection<OptimizedData> LoadsFromPowerPlants { get; set; }

        public OptimizationPerHour()
        {
            LoadsFromPowerPlants = new List<OptimizedData>();
        }
    }
}
