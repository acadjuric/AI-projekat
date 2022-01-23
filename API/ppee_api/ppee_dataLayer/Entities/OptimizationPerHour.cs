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
        public DateTime DateTimeOfOptimization { get; set; }
        //from AI, predicted load
        public int Load { get; set; }
        
        public double Price { get; set; }
        public double Emission { get; set; }
        //sum from optimizedData collection
        public double LoadSum { get; set; }

        public double SolarLoad { get; set; }
        public double WindLoad { get; set; }
        public double HydroLoad { get; set; }
        public double CoalLoad { get; set; }
        public double GasLoad { get; set; }

        public List<OptimizedData> LoadsFromPowerPlants { get; set; }

        public OptimizationPerHour()
        {
            LoadsFromPowerPlants = new List<OptimizedData>();
        }
    }
}
