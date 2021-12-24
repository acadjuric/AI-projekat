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
        #region static Min Max properties
        public static float minAirTemperature { get; set; }
        public static float maxAirTemperature { get; set; }
        public static float minHumidity { get; set; }
        public static float maxHumidity { get; set; }
        public static float minAtmosphericPressure { get; set; }
        public static float maxAtmosphericPressure { get; set; }
        public static float minPressureTendency { get; set; }
        public static float maxPressureTendency { get; set; }
        public static float minMeanWindSpeed { get; set; }
        public static float maxMeanWindSpeed { get; set; }
        public static float minMaxGustValue { get; set; }
        public static float maxMaxGustValue { get; set; }
        public static float minTotalCloudCover { get; set; }
        public static float maxTotalCloudCover { get; set; }
        public static float minVisibility { get; set; }
        public static float maxVisibility { get; set; }
        public static float minDewPointTemperature { get; set; }
        public static float maxDewPointTemperature { get; set; }
        public static float minMWh { get; set; }
        public static float maxMWh { get; set; }
        #endregion

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

        public static List<List<float>> ScaleDataSetAndGetPredictorData(List<WeatherAndLoad> data)
        {
            minAirTemperature = data.Min(x => x.AirTemperature);
            maxAirTemperature = data.Max(x => x.AirTemperature);

            minHumidity = data.Min(x => x.RelativeHumidity);
            maxHumidity = data.Max(x => x.RelativeHumidity);

            minAtmosphericPressure = data.Min(x => x.AtmosphericPressure);
            maxAtmosphericPressure = data.Max(x => x.AtmosphericPressure);

            minPressureTendency = data.Min(x => x.PressureTendency);
            maxPressureTendency = data.Max(x => x.PressureTendency);

            minMeanWindSpeed = data.Min(x => x.MeanWindSpeed);
            maxMeanWindSpeed = data.Max(x => x.MeanWindSpeed);

            minMaxGustValue = data.Min(x => x.MaxGustValue);
            maxMaxGustValue = data.Max(x => x.MaxGustValue);

            minTotalCloudCover = data.Min(x => x.TotalCloudCover);
            maxTotalCloudCover = data.Max(x => x.TotalCloudCover);

            minVisibility = data.Min(x => x.Visibility);
            maxVisibility = data.Max(x => x.Visibility);

            minDewPointTemperature = data.Min(x => x.DewPointTemperature);
            maxDewPointTemperature = data.Max(x => x.DewPointTemperature);

            minMWh = data.Min(x => x.MWh);
            maxMWh = data.Max(x => x.MWh);


            // ZA OVO NE TREBA TRAZITI MIN I MAX
            // dayOfWeek -> min:0 ; max:6 ; 0-nedelja ... 6-subota
            // month -> min:1 ; max:12 ; 1-januar ... 12-decembar
            // hour -> min:0 ; max:23

            List<List<float>> predictorData = new List<List<float>>();

            foreach (var item in data)
            {
                List<float> rowData = new List<float>();
                //predictor
                item.AirTemperature = NormalizeData(item.AirTemperature, minAirTemperature, maxAirTemperature);
                item.AtmosphericPressure = NormalizeData(item.AtmosphericPressure, minAtmosphericPressure, maxAtmosphericPressure);
                item.PressureTendency = NormalizeData(item.PressureTendency, minPressureTendency, maxPressureTendency);
                item.MeanWindSpeed = NormalizeData(item.MeanWindSpeed, minMeanWindSpeed, maxMeanWindSpeed);
                item.MaxGustValue = NormalizeData(item.MaxGustValue, minMaxGustValue, maxMaxGustValue);
                item.RelativeHumidity = NormalizeData(item.RelativeHumidity, minHumidity, maxHumidity);
                item.TotalCloudCover = NormalizeData(item.TotalCloudCover, minTotalCloudCover, maxTotalCloudCover);
                item.Visibility = NormalizeData(item.Visibility, minVisibility, maxVisibility);
                item.DewPointTemperature = NormalizeData(item.DewPointTemperature, minDewPointTemperature, maxDewPointTemperature);
                item.Hour = NormalizeData(item.Hour, 0, 23);
                item.Month = NormalizeData(item.Month, 1, 12);
                item.DayOfWeek = NormalizeData(item.DayOfWeek, 0, 6);

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
                item.MWh = NormalizeData(item.MWh, minMWh, maxMWh);
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

            return temp.Average();
        }

        public static List<float> InverseTransform_MWh(List<float> scaledData)
        {
            List<float> retVal = new List<float>();
            foreach (var item in scaledData)
            {
                retVal.Add(DeNormalizeData(item, minMWh, maxMWh));
            }

            return retVal;
        }
    }
}
