using Numpy;
using ppee_dataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_service.MyKerasEntity
{
    public static class KerasHelpers
    {
        
        public static List<float> NDarrayToList(NDarray array)
        {
            List<float> retVal = new List<float>();

            for (int i = 0; i < array.len; i++)
                retVal.Add((float)array[i][0]);

            return retVal;
        }

        public static float[,] CreateMatrix(List<List<float>> predictorVariables, int rowCount, int columnCount)
        {
            float[,] predVar = new float[rowCount, columnCount];
            for (int i = 0; i < predictorVariables.Count; i++)
            {
                for (int j = 0; j < predictorVariables[i].Count; j++)
                {
                    predVar[i, j] = predictorVariables[i][j];
                }
            }

            return predVar;
        }

        public static float NormalizeData(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        public static float DeNormalizeData(float normalized, float min, float max)
        {
            return (normalized * (max - min) + min);
        }

        public static MinMaxValues FindMinAndMaxValues(List<WeatherAndLoad> data)
        {
            MinMaxValues minMaxValues = new MinMaxValues();

            minMaxValues.MinAirTemperature = data.Min(x => x.AirTemperature);
            minMaxValues.MaxAirTemperature = data.Max(x => x.AirTemperature);

            minMaxValues.MinHumidity = data.Min(x => x.RelativeHumidity);
            minMaxValues.MaxHumidity = data.Max(x => x.RelativeHumidity);

            minMaxValues.MinAtmosphericPressure = data.Min(x => x.AtmosphericPressure);
            minMaxValues.MaxAtmosphericPressure = data.Max(x => x.AtmosphericPressure);

            minMaxValues.MinPressureTendency = data.Min(x => x.PressureTendency);
            minMaxValues.MaxPressureTendency = data.Max(x => x.PressureTendency);

            minMaxValues.MinMeanWindSpeed = data.Min(x => x.MeanWindSpeed);
            minMaxValues.MaxMeanWindSpeed = data.Max(x => x.MeanWindSpeed);

            minMaxValues.MinMaxGustValue = data.Min(x => x.MaxGustValue);
            minMaxValues.MaxMaxGustValue = data.Max(x => x.MaxGustValue);

            minMaxValues.MinTotalCloudCover = data.Min(x => x.TotalCloudCover);
            minMaxValues.MaxTotalCloudCover = data.Max(x => x.TotalCloudCover);

            minMaxValues.MinVisibility = data.Min(x => x.Visibility);
            minMaxValues.MaxVisibility = data.Max(x => x.Visibility);

            minMaxValues.MinDewPointTemperature = data.Min(x => x.DewPointTemperature);
            minMaxValues.MaxDewPointTemperature = data.Max(x => x.DewPointTemperature);

            minMaxValues.MinMWh = data.Min(x => x.MWh);
            minMaxValues.MaxMWh = data.Max(x => x.MWh);

            // ZA OVO NE TREBA TRAZITI MIN I MAX
            // dayOfWeek -> min:0 ; max:6 ; 0-nedelja ... 6-subota
            // month -> min:1 ; max:12 ; 1-januar ... 12-decembar
            // hour -> min:0 ; max:23

            minMaxValues.MinHour = 0;
            minMaxValues.MaxHour = 23;
            
            minMaxValues.MinMonth = 1;
            minMaxValues.MaxMonth = 12;

            minMaxValues.MinDayOfWeek = 0; //nedelja
            minMaxValues.MaxDayOfWeek = 6; //subota

            return minMaxValues;
        }

        public static List<List<float>> ScaleDataSetAndGetPredictorData(List<WeatherAndLoad> data, MinMaxValues minMaxValues)
        {
            
            List<List<float>> predictorData = new List<List<float>>();

            foreach (var item in data)
            {
                List<float> rowData = new List<float>();
                //predictor
                item.AirTemperature = NormalizeData(item.AirTemperature, minMaxValues.MinAirTemperature, minMaxValues.MaxAirTemperature);
                item.AtmosphericPressure = NormalizeData(item.AtmosphericPressure, minMaxValues.MinAtmosphericPressure, minMaxValues.MaxAtmosphericPressure);
                item.PressureTendency = NormalizeData(item.PressureTendency, minMaxValues.MinPressureTendency, minMaxValues.MaxPressureTendency);
                item.MeanWindSpeed = NormalizeData(item.MeanWindSpeed, minMaxValues.MinMeanWindSpeed, minMaxValues.MaxMeanWindSpeed);
                item.MaxGustValue = NormalizeData(item.MaxGustValue, minMaxValues.MinMaxGustValue, minMaxValues.MaxMaxGustValue);
                item.RelativeHumidity = NormalizeData(item.RelativeHumidity, minMaxValues.MinHumidity, minMaxValues.MaxHumidity);
                item.TotalCloudCover = NormalizeData(item.TotalCloudCover, minMaxValues.MinTotalCloudCover, minMaxValues.MaxTotalCloudCover);
                item.Visibility = NormalizeData(item.Visibility, minMaxValues.MinVisibility, minMaxValues.MaxVisibility);
                item.DewPointTemperature = NormalizeData(item.DewPointTemperature, minMaxValues.MinDewPointTemperature, minMaxValues.MaxDewPointTemperature);
                item.Hour = NormalizeData(item.Hour, minMaxValues.MinHour, minMaxValues.MaxHour);
                item.Month = NormalizeData(item.Month, minMaxValues.MinMonth, minMaxValues.MaxMonth);
                item.DayOfWeek = NormalizeData(item.DayOfWeek, minMaxValues.MinDayOfWeek, minMaxValues.MaxDayOfWeek);

                rowData.Add(item.AirTemperature);
                rowData.Add(item.AtmosphericPressure);
                rowData.Add(item.PressureTendency);
                rowData.Add(item.MeanWindSpeed);
                rowData.Add(item.MaxGustValue);
                rowData.Add(item.RelativeHumidity);
                rowData.Add(item.TotalCloudCover);
                rowData.Add(item.Visibility);
                rowData.Add(item.DewPointTemperature);
                rowData.Add(item.Hour);
                rowData.Add(item.Month);
                rowData.Add(item.DayOfWeek);

                predictorData.Add(rowData);

                //predicted
                item.MWh = NormalizeData(item.MWh, minMaxValues.MinMWh, minMaxValues.MaxMWh);
            }

            return predictorData;
        }

        public static double GetSquareDeviation(List<float> forecastValues, List<float> actualValues)
        {
            //RMSE - Root Mean Square Error
            if (forecastValues.Count != actualValues.Count)
                throw new Exception("Different lenghts");

            List<double> deviations = new List<double>();
            for (int i = 0; i < forecastValues.Count; i++)
            {
                deviations.Add(Math.Pow((double)(new decimal(forecastValues[i])) - (double)(new decimal(actualValues[i])), 2));
            }
            return Math.Sqrt(deviations.Average());
        }

        public static double GetAbsoluteDeviation(List<float> forecastValues, List<float> actualValues)
        {
            //MAPE - mean absolute percentage error

            if (forecastValues.Count != actualValues.Count)
                throw new Exception("Different lenghts");

            List<float> temp = new List<float>();
            for (int i = 0; i < forecastValues.Count; i++)
            {
                temp.Add(Math.Abs((actualValues[i] - forecastValues[i]) / actualValues[i]));
            }

            return temp.Average() * 100;
        }

        public static List<float> InverseTransform_MWh(List<float> scaledData, MinMaxValues minMaxValues)
        {
            List<float> retVal = new List<float>();
            foreach (var item in scaledData)
            {
                retVal.Add(DeNormalizeData(item, minMaxValues.MinMWh, minMaxValues.MaxMWh));
            }

            return retVal;
        }
    }
}
