using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Entities
{
    public class MinMaxValues
    {
        public MinMaxValues()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public float MinAirTemperature { get; set; }
        public float MaxAirTemperature { get; set; }

        public float MinHumidity { get; set; }
        public float MaxHumidity { get; set; }

        public float MinAtmosphericPressure { get; set; }
        public float MaxAtmosphericPressure { get; set; }

        public float MinPressureTendency { get; set; }
        public float MaxPressureTendency { get; set; }

        public float MinMeanWindSpeed { get; set; }
        public float MaxMeanWindSpeed { get; set; }

        public float MinMaxGustValue { get; set; }
        public float MaxMaxGustValue { get; set; }

        public float MinTotalCloudCover { get; set; }
        public float MaxTotalCloudCover { get; set; }

        public float MinVisibility { get; set; }
        public float MaxVisibility { get; set; }

        public float MinDewPointTemperature { get; set; }
        public float MaxDewPointTemperature { get; set; }

        public float MinMWh { get; set; }
        public float MaxMWh { get; set; }

        public float MinHour { get; set; } = 0;
        public float MaxHour { get; set; } = 23;

        public float MinMonth { get; set; } = 1;
        public float MaxMonth { get; set; } = 12;

        public float MinDayOfWeek { get; set; } = 0; //nedelja
        public float MaxDayOfWeek { get; set; } = 6; //subota


        


    }
}
