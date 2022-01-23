using CenterSpace.NMath.Core;
using Newtonsoft.Json;
using ppee_dataLayer.Entities;
using ppee_dataLayer.Interfaces;
using ppee_dataLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ppee_service.Optimization
{
    public class Optimization
    {
        public Optimization()
        {

        }

        //private List<PowerPlant> InitElektrane()
        //{
        //    List<PowerPlant> elektrane = new List<PowerPlant>();

        //    PowerPlant e1 = new PowerPlant() { Type = "hidro", MaximumOutputPower = 4000, MinimumOutputPower = 10 };
        //    PowerPlant e2 = new PowerPlant() { Type = "hidro", MaximumOutputPower = 2000, MinimumOutputPower = 15 };
        //    PowerPlant e3 = new PowerPlant() { Type = "solar", Efficiency = 40, SurfaceArea = 150 };
        //    PowerPlant e4 = new PowerPlant() { Type = "solar", Efficiency = 18, SurfaceArea = 120 };
        //    PowerPlant e5 = new PowerPlant() { Type = "wind", BladesSweptAreaDiameter = 40, NumberOfWindGenerators = 30 };
        //    PowerPlant e6 = new PowerPlant() { Type = "wind", BladesSweptAreaDiameter = 75, NumberOfWindGenerators = 10 };
        //    PowerPlant e7 = new PowerPlant() { Type = "coal", MaximumOutputPower = 300, MinimumOutputPower = 70 };
        //    PowerPlant e8 = new PowerPlant() { Type = "coal", MaximumOutputPower = 500, MinimumOutputPower = 100 };
        //    PowerPlant e9 = new PowerPlant() { Type = "gas", MaximumOutputPower = 250, MinimumOutputPower = 90 };
        //    PowerPlant e10 = new PowerPlant() { Type = "gas", MaximumOutputPower = 340, MinimumOutputPower = 130 };

        //    elektrane.Add(e1); elektrane.Add(e2); elektrane.Add(e3); elektrane.Add(e4); elektrane.Add(e5);
        //    elektrane.Add(e6); elektrane.Add(e7); elektrane.Add(e8); elektrane.Add(e9); elektrane.Add(e10);

        //    return elektrane;
        //}



        public async Task<List<OptimizationPerHour>> CreateOptimization(OptimizationSettings optimizationSettings)
        {
            string date = optimizationSettings.Date;
            string startDateTime = date + " 00:00";
            string endDateTme = date + " 23:00";

            string apiKey = "c4948a701804bfb438573ae542b49f47";
            double lat = 45.267136;
            double lon = 19.833549;
            long time = ((DateTimeOffset)DateTime.Now.AddDays(-4)).ToUnixTimeSeconds();
            string result = string.Empty;

            IDatabase dataSloj = new DatabaseService();

            List<ForecastValues> forecastValues = await dataSloj.LoadPredictedValues();

            List<ForecastValues> predictionHours = forecastValues.Where(x => DateTime.Parse(x.DateAndTime) >= DateTime.Parse(startDateTime) && DateTime.Parse(x.DateAndTime) <= DateTime.Parse(endDateTme)).ToList();

            try
            {
                string api = $"https://api.openweathermap.org/data/2.5/onecall/timemachine?lat={lat}&lon={lon}&dt={time}&appid={apiKey}";
                WebClient webClient = new WebClient();
                result = webClient.DownloadString(api);
            }
            catch { return null; }

            var jsonResult = JsonConvert.DeserializeObject<dynamic>(result);

            List<double> sunAnglesForHours = GetSunAngleInRadiansForHours(jsonResult);
            List<CloudsAndWindSpeed> cloudsAndWindSpeeds = GetCloudsAndWindSpeedFromJson(jsonResult);


            List<OptimizationPerHour> dayOptimization = new List<OptimizationPerHour>();

            for (int i = 0; i < predictionHours.Count; i++)
            {
                //za svaki sat pozivamo optimizaciju
                OptimizationPerHour optimizationPerHour = new OptimizationPerHour();

                optimizationPerHour.DateTimeOfOptimization = DateTime.Parse(predictionHours[i].DateAndTime);
                optimizationPerHour.Load = (int)predictionHours[i].Load;

                List<OptimizedData> optimizedData = OptimizationPerHour(predictionHours[i], optimizationSettings, cloudsAndWindSpeeds[i].WindSpeed, cloudsAndWindSpeeds[i].WindSpeed, sunAnglesForHours[i]);

                //optimizationPerHour.LoadsFromPowerPlants = optimizedData;

                foreach (OptimizedData od in optimizedData)
                {
                    od.OptimizationPerHour = optimizationPerHour;
                    optimizationPerHour.LoadsFromPowerPlants.Add(od);
                }

                //upis u bazu
                dayOptimization.Add(optimizationPerHour);
            }

            return dayOptimization;
        }



        private List<OptimizedData> OptimizationPerHour(ForecastValues forecast, OptimizationSettings optimizationSettings, double windSpeed, double clouds, double sunAngle)
        {
            List<OptimizedData> optimizedData = new List<OptimizedData>();

            int sunPower = 0;
            int windPower = 0;
            int hydroPower = 0;

            int renewableSources = 0;
            int minimumOutputPowerFromNonRenewableSources = 0;

            foreach (PowerPlant powerPlant in optimizationSettings.PowerPlantsForOptimization)
            {
                if (powerPlant.Type.ToLower().Equals("hydro"))
                {
                    hydroPower += powerPlant.MaximumOutputPower;
                    renewableSources += powerPlant.MaximumOutputPower;

                    OptimizedData od = new OptimizedData()
                    {
                        Name = powerPlant.Name + "-" + powerPlant.Id.ToString(),
                        Load = powerPlant.MaximumOutputPower,
                        Type = powerPlant.Type,
                        CO2 = 0,
                        Cost = (int)(powerPlant.MaximumOutputPower * 0.4),
                    };

                    optimizedData.Add(od);

                }
                else if (powerPlant.Type.ToLower().Equals("solar"))
                {
                    //double konacnaSnaga = ((0.1 + (0.9 * clouds)) * powerPlant.MaximumOutputPower * (sunAngle * Imax) / 1000000; //Megawatt

                    //double konacnaSnaga = ((0.1 + 0.9 * (Math.Abs(1 - (clouds / 100)))) * (sunAngle * Imax) * powerPlant.MaximumOutputPower) / 1000000;
                    int tempPower = (int)((0.1 + 0.9 * clouds) * (sunAngle) * powerPlant.MaximumOutputPower) / 1000000; //Megawatt
                    renewableSources += tempPower;
                    sunPower += tempPower;

                    OptimizedData od = new OptimizedData()
                    {
                        Name = powerPlant.Name + "-" + powerPlant.Id.ToString(),
                        Load = tempPower,
                        Type = powerPlant.Type,
                        CO2 = 0,
                        Cost = 0,
                    };

                    optimizedData.Add(od);
                }
                else if (powerPlant.Type.ToLower().Equals("wind"))
                {
                    int tempPower = 0;
                    if (windSpeed < 2 || windSpeed > 25)
                    {
                        tempPower = 0;
                        renewableSources += tempPower;
                        windPower += tempPower;
                    }
                    else if (windSpeed >= 12 || windSpeed <= 25)
                    {
                        tempPower = (int)(powerPlant.MaximumOutputPower * powerPlant.NumberOfWindGenerators) / 1000;
                        renewableSources += tempPower;
                        windPower += tempPower;
                    }
                    else
                    {
                        tempPower = (int)WindFunction(windSpeed, powerPlant.MaximumOutputPower);
                        tempPower = (int)(tempPower * powerPlant.NumberOfWindGenerators) / 1000;
                        renewableSources += tempPower;
                        windPower += tempPower;
                    }

                    OptimizedData od = new OptimizedData()
                    {
                        Name = powerPlant.Name + "-" + powerPlant.Id.ToString(),
                        Load = tempPower,
                        Type = powerPlant.Type,
                        CO2 = 0,
                        Cost = 0,
                    };

                    optimizedData.Add(od);
                }
                else
                {
                    minimumOutputPowerFromNonRenewableSources += powerPlant.MinimumOutputPower;
                }
            }

            int coefficient = 1;
            if (renewableSources > forecast.Load - minimumOutputPowerFromNonRenewableSources)
            {
                coefficient = (int)((forecast.Load - minimumOutputPowerFromNonRenewableSources) / renewableSources);
            }
            foreach (OptimizedData od in optimizedData)
            {
                od.Load *= coefficient;
            }
            renewableSources = (int)optimizedData.Sum(x => x.Load);

            int LoadForNonRenewablePowerPlants = (int)(forecast.Load - renewableSources);
            List<PowerPlant> nonRenewablePowerPlants = optimizationSettings.PowerPlantsForOptimization.Where(x => x.Type.ToLower().Equals("gas") || x.Type.ToLower().Equals("coal")).ToList();

            if (optimizationSettings.OptimizationType.ToLower().Equals("both"))
            {
                optimizationSettings.OptimizationType = "cost";
                List<OptimizedData> nonRenewableOptimizedDataCost = OptimizationForNonRenewable(optimizationSettings, nonRenewablePowerPlants, LoadForNonRenewablePowerPlants);

                optimizationSettings.OptimizationType = "co2";
                List<OptimizedData> nonRenewableOptimizedDataCO2 = OptimizationForNonRenewable(optimizationSettings, nonRenewablePowerPlants, LoadForNonRenewablePowerPlants);

                double weightFactorCO2 = 1 - optimizationSettings.WeightFactor;

                for (int i = 0; i < nonRenewablePowerPlants.Count; i++)
                {
                    //isti broj elementa imaju pa moze u jednom for-u
                    nonRenewableOptimizedDataCost[i].Load *= optimizationSettings.WeightFactor;
                    nonRenewableOptimizedDataCO2[i].Load *= weightFactorCO2;

                    nonRenewableOptimizedDataCost[i].Load += nonRenewableOptimizedDataCO2[i].Load;

                    Tuple<int, int> costAndCO2emission = CalculateFuleCostAndCO2Emission(optimizationSettings, nonRenewablePowerPlants[i], nonRenewableOptimizedDataCost[i].Load);
                    nonRenewableOptimizedDataCost[i].Cost = costAndCO2emission.Item1;
                    nonRenewableOptimizedDataCost[i].CO2 = costAndCO2emission.Item2;
                }

                optimizedData.AddRange(nonRenewableOptimizedDataCost);

            }
            else
            {
                List<OptimizedData> nonRenewableOptimizedData = OptimizationForNonRenewable(optimizationSettings, nonRenewablePowerPlants, LoadForNonRenewablePowerPlants);

                optimizedData.AddRange(nonRenewableOptimizedData);
            }




            return optimizedData;
        }

        private List<OptimizedData> OptimizationForNonRenewable(OptimizationSettings optimizationSettings, List<PowerPlant> powerPlants, double load)
        {
            // Z = MAX( -1X - 2Y)
            DoubleVector costFunction = new DoubleVector();

            //SVAKI GENERATOR JE POSEBAN ZA SEBE
            // X + Y + Z + U + V + M + N = SAMO ONO STO NEOBNOVLJIVI TREBA DA PROIZVODE [LOAD]
            DoubleVector allPowerPlants = new DoubleVector();

            foreach (PowerPlant powerPlant in powerPlants)
            {
                if (optimizationSettings.OptimizationType.ToLower().Equals("cost"))
                {
                    if (powerPlant.Type.ToLower().Equals("coal"))
                    {
                        costFunction.Append(-optimizationSettings.CostCoal);
                    }
                    else if (powerPlant.Type.ToLower().Equals("gas"))
                    {
                        costFunction.Append(-optimizationSettings.CostGas);
                    }
                }
                else if (optimizationSettings.OptimizationType.ToLower().Equals("co2"))
                {
                    if (powerPlant.Type.ToLower().Equals("coal"))
                    {
                        costFunction.Append(-optimizationSettings.CO2Coal);
                    }
                    else if (powerPlant.Type.ToLower().Equals("gas"))
                    {
                        costFunction.Append(-optimizationSettings.CO2Gas);
                    }
                }

                allPowerPlants.Append(1);

            }

            LinearProgrammingProblem linearProgrammingProblem = new LinearProgrammingProblem(costFunction);

            linearProgrammingProblem.AddEqualityConstraint(allPowerPlants, load);

            for (int i = 0; i < powerPlants.Count; i++)
            {
                DoubleVector maxAndMinOutputForPowerPlant = new DoubleVector();

                for (int j = 0; j < powerPlants.Count; j++)
                {
                    if (i == j)
                        maxAndMinOutputForPowerPlant.Append(1);
                    else
                        maxAndMinOutputForPowerPlant.Append(0);
                }

                linearProgrammingProblem.AddLowerBoundConstraint(maxAndMinOutputForPowerPlant, powerPlants[i].MinimumOutputPower);
                linearProgrammingProblem.AddUpperBoundConstraint(maxAndMinOutputForPowerPlant, powerPlants[i].MaximumOutputPower);
            }

            PrimalSimplexSolver simplex = new PrimalSimplexSolver();
            simplex.Solve(linearProgrammingProblem);

            DoubleVector optimalSolution = simplex.OptimalX;

            List<OptimizedData> nonRenewableOptimizedData = new List<OptimizedData>();

            for (int i = 0; i < powerPlants.Count; i++)
            {
                OptimizedData od = new OptimizedData()
                {
                    Name = powerPlants[i].Name + "-" + powerPlants[i].Id.ToString(),
                    Load = optimalSolution[i],
                    Type = powerPlants[i].Type,
                    CO2 = 0,
                    Cost = 0,
                };


                Tuple<int, int> costAndCO2emission = CalculateFuleCostAndCO2Emission(optimizationSettings, powerPlants[i], od.Load);
                od.Cost = costAndCO2emission.Item1;
                od.CO2 = costAndCO2emission.Item2;


                nonRenewableOptimizedData.Add(od);
            }

            return nonRenewableOptimizedData;
        }

        private Tuple<int, int> CalculateFuleCostAndCO2Emission(OptimizationSettings optimizationSettings, PowerPlant powerPlant, double load)
        {
            int cost = 0;
            int co2emission = 0;

            double x = Scale(load, powerPlant.MinimumOutputPower, powerPlant.MaximumOutputPower, 8, 16);
            if (powerPlant.Type.ToLower().Equals("gas"))
            {
                cost = (int)((8 / x) * optimizationSettings.CostGas * load);
                co2emission = (int)((0.5 + Math.Pow((x / 5), 2)) * optimizationSettings.CO2Gas * load);
            }
            else
            {
                cost = (int)((8 / x) * optimizationSettings.CostCoal * load);
                co2emission = (int)((0.5 + Math.Pow((x / 5), 2)) * optimizationSettings.CO2Coal * load);
            }

            return new Tuple<int, int>(cost, co2emission);

        }

        private double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }

        private double WindFunction(double mojaXvrednost, double PmaxWind)
        {
            double prvaX = 2;
            double prvaY = 0;
            double drugaX = 12;

            // y - y1 = ( (y2-y1) / (x2-x1) ) * (x - x1)

            return ((PmaxWind - prvaY) / (drugaX - prvaX)) * (mojaXvrednost - prvaX) + prvaY;
        }

        private List<double> GetSunAngleInRadiansForHours(dynamic jsonResult)
        {
            Tuple<int, int> sunriseAndSunsetHours = GetSunriseAndSunsetHours(Convert.ToInt64(jsonResult.current.sunrise), Convert.ToInt64(jsonResult.current.sunset));

            int sunriseHour = sunriseAndSunsetHours.Item1;
            int sunsetHour = sunriseAndSunsetHours.Item2;

            List<int> hours = new List<int>();
            for (int i = 0; i < 24; i++)
            {
                if (i < sunriseHour || i > sunsetHour)
                    hours.Add(0);
                else
                {
                    if (i > 12)
                        hours.Add(12 - (i - 12));
                    else
                        hours.Add(i);
                }
            }

            List<double> hoursForSinusInput = new List<double>();
            hours.ForEach(x => hoursForSinusInput.Add(ScaleBetween(x, 0, Math.PI / 2, 0, 12)));

            List<double> anglesFromSinus = new List<double>();
            foreach (var scaledHour in hoursForSinusInput)
            {
                anglesFromSinus.Add(Math.Sin(scaledHour));
            }

            return anglesFromSinus;
        }

        private Tuple<int, int> GetSunriseAndSunsetHours(long unixSunrise, long unixSunset)
        {
            DateTimeOffset SunRiseDate = DateTimeOffset.FromUnixTimeSeconds(unixSunrise);
            DateTimeOffset sunSetDate = DateTimeOffset.FromUnixTimeSeconds(unixSunset);

            int sunriseHour = SunRiseDate.DateTime.Hour;
            int sunriseMinutes = SunRiseDate.DateTime.Minute;

            int sunsetHour = sunSetDate.DateTime.Hour;
            int sunsetMinute = sunSetDate.DateTime.Minute;

            if (sunriseMinutes > 30) sunriseHour++;
            if (sunsetMinute > 30) sunsetHour++;

            return new Tuple<int, int>(sunriseHour, sunsetHour);
        }

        private List<CloudsAndWindSpeed> GetCloudsAndWindSpeedFromJson(dynamic jsonResult)
        {
            List<CloudsAndWindSpeed> cloudsAndWindSpeeds = new List<CloudsAndWindSpeed>();

            foreach (var item in jsonResult.hourly)
            {
                CloudsAndWindSpeed temp = new CloudsAndWindSpeed()
                {
                    Clouds = item.clouds,
                    WindSpeed = item.wind_speed,
                };

                cloudsAndWindSpeeds.Add(temp);
            }

            return cloudsAndWindSpeeds;
        }

        private double ScaleBetween(double unscaledNum, double minAllowed, double maxAllowed, double min, double max)
        {
            return (maxAllowed - minAllowed) * (unscaledNum - min) / (max - min) + minAllowed;
        }


    }
}
