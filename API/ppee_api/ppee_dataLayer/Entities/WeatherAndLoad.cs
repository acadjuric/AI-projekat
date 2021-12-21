using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Entities
{
    public class WeatherAndLoad
    {
        public WeatherAndLoad()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Date { get; set; }
        public double AirTemperature { get; set; }
        public double AtmosphericPressure { get; set; }
        public double PressureTendency { get; set; }
        public double RelativeHumidity { get; set; }
        public double MeanWindSpeed { get; set; }
        public double MaxGustValue { get; set; }
        public double TotalCloudCover { get; set; }
        public double Visibility { get; set; }
        public double DewPointTemperature { get; set; }
        public long MWh { get; set; }
    }
}
