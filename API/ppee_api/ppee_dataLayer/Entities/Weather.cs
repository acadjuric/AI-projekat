using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Entities
{
    public class Weather
    {

        public Weather()
        {

        }

        
       

        public string Date { get; set; }
        public string AirTemperature { get; set; }
        public string AtmosphericPressure { get; set; }
        public string PressureTendency { get; set; }
        public string RelativeHumidity { get; set; }
        public string MeanWindSpeed { get; set; }
        public string MaxGustValue { get; set; }
        public string TotalCloudCover { get; set; }
        public string Visibility { get; set; }
        public string DewPointTemperature { get; set; }
        
    }
}
