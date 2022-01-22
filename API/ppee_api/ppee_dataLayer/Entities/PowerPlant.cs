using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Entities
{
    public class PowerPlant
    {
        public int MaximumOutputPower { get; set; } = 0;
        public int MinimumOutputPower { get; set; } = 0;
        public int Efficiency { get; set; } = 0;
        public int SurfaceArea { get; set; } = 0;
        public int BladesSweptAreaDiameter { get; set; } = 0;
        public int NumberOfWindGenerators { get; set; } = 0;
        public string Type { get; set; }

        public PowerPlant()
        {
            
        }

    }
}
