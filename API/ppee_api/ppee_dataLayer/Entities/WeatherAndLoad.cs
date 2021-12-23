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
        public float DayOfWeek { get; set; }
        public float Hour { get; set; }
        public float Month { get; set; }
        public float AirTemperature { get; set; }
        public float AtmosphericPressure { get; set; }
        public float PressureTendency { get; set; }
        public float RelativeHumidity { get; set; }
        public float MeanWindSpeed { get; set; }
        public float MaxGustValue { get; set; }
        public float TotalCloudCover { get; set; }
        public float Visibility { get; set; }
        public float DewPointTemperature { get; set; }
        public float MWh { get; set; }
    }
}
