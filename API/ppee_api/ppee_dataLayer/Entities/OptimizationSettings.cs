using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Entities
{
    public class OptimizationSettings
    {
        public string Date { get; set; }
        public string OptimizationType { get; set; }
        public int CostCoal { get; set; }
        public int CostGas { get; set; }
        public int CO2Gas { get; set; }
        public double CO2Coal { get; set; }
        public double WeightFactor { get; set; }
        public List<PowerPlant> PowerPlantsForOptimization { get; set; }
        public List<IdAndNumber> PowerPlantAndNumberForOptimization { get; set; }

        public OptimizationSettings()
        {

        }

    }
}
