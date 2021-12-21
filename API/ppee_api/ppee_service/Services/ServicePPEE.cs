using ExcelDataReader;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using ppee_dataLayer.Entities;
using ppee_dataLayer.Interfaces;
using ppee_dataLayer.Services;
using ppee_service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ppee_service.Services
{
    public class ServicePPEE : IServicePPEE
    {
        #region PomocnePromenljive
        int brojac = 0;
        int prethodnaVrednostZaCloudCoverage = -1;
        string prethodnaAirTemperature = string.Empty;
        string prethodnaAtmosphericPressure = string.Empty;
        string prethodnaPressureTendency = string.Empty;
        string prethodnaRelativeHumidity = string.Empty;
        string prethodnaMeanWindSPeed = string.Empty;
        string prethodnaMaxGustValue = string.Empty;
        string prethodnaPresentWeather = string.Empty;
        string prethodnaVisibility = string.Empty;
        string prethondaDewPointTemperature = string.Empty;
        #endregion


        public async Task<bool> ReadFile(Stream stream)
        {

            IDatabase dataSloj = new DatabaseService();

            try
            {
                //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                });

                var sheets = result.Tables;
                List<Load> potorsnja = new List<Load>();
                List<Weather> weathers = new List<Weather>();
                List<Weather> weathersWithEmptyFields = new List<Weather>();

                Dictionary<string, List<int>> propertiesWithEmptyFields = new Dictionary<string, List<int>>()
                {
                    {"T", new List<int>() },{"Po", new List<int>() },{"Pa", new List<int>() },{"U", new List<int>() },{"Ff", new List<int>() },
                    {"ff3", new List<int>() },{"N", new List<int>() },{"WW", new List<int>() },{"VV", new List<int>() },{"Td", new List<int>() },
                };

                //Regex pattern = new Regex("\\D+");

                foreach (DataTable sheet in sheets)
                {
                    if (sheet.TableName.ToLower().Equals("weather"))
                    {

                        foreach (DataRow row in sheet.Rows)
                        {
                            Weather w = new Weather();
                            w.Date = row["Local time"].ToString(); //dan.mesec.godina sat:minut
                            if (w.Date.Equals(string.Empty)) continue;

                            w.AirTemperature = row["T"].ToString().Equals(string.Empty) ? prethodnaAirTemperature : row["T"].ToString();
                            prethodnaAirTemperature = w.AirTemperature;
                            if (prethodnaAirTemperature.Equals(string.Empty))
                                propertiesWithEmptyFields["T"].Add(weathers.Count);
                            else if (propertiesWithEmptyFields["T"].Count > 0)
                                FixEmptyFields(propertiesWithEmptyFields, weathers, "T");

                            w.AtmosphericPressure = row["Po"].ToString().Equals(string.Empty) ? prethodnaAtmosphericPressure : row["Po"].ToString();
                            prethodnaAtmosphericPressure = w.AtmosphericPressure;
                            if (prethodnaAtmosphericPressure.Equals(string.Empty))
                                propertiesWithEmptyFields["Po"].Add(weathers.Count);
                            else if (propertiesWithEmptyFields["Po"].Count > 0)
                                FixEmptyFields(propertiesWithEmptyFields, weathers, "Po");

                            w.PressureTendency = row["Pa"].ToString().Equals(string.Empty) ? prethodnaPressureTendency : row["Pa"].ToString(); ;
                            prethodnaPressureTendency = w.PressureTendency;
                            if (prethodnaPressureTendency.Equals(string.Empty))
                                propertiesWithEmptyFields["Pa"].Add(weathers.Count);
                            else if (propertiesWithEmptyFields["Pa"].Count > 0)
                                FixEmptyFields(propertiesWithEmptyFields, weathers, "Pa");

                            w.RelativeHumidity = row["U"].ToString().Equals(string.Empty) ? prethodnaRelativeHumidity : row["U"].ToString(); ;
                            prethodnaRelativeHumidity = w.RelativeHumidity;
                            if (prethodnaRelativeHumidity.Equals(string.Empty))
                                propertiesWithEmptyFields["U"].Add(weathers.Count);
                            else if (propertiesWithEmptyFields["U"].Count > 0)
                                FixEmptyFields(propertiesWithEmptyFields, weathers, "U");

                            w.MeanWindSpeed = row["Ff"].ToString().Equals(string.Empty) ? prethodnaMeanWindSPeed : row["Ff"].ToString();
                            prethodnaMeanWindSPeed = w.MeanWindSpeed;
                            if (prethodnaMeanWindSPeed.Equals(string.Empty))
                                propertiesWithEmptyFields["Ff"].Add(weathers.Count);
                            else if (propertiesWithEmptyFields["Ff"].Count > 0)
                                FixEmptyFields(propertiesWithEmptyFields, weathers, "Ff");

                            w.MaxGustValue = row["ff3"].ToString().Equals(string.Empty) ? prethodnaMaxGustValue : row["ff3"].ToString();
                            prethodnaMaxGustValue = w.MaxGustValue;
                            if (prethodnaMaxGustValue.Equals(string.Empty))
                                propertiesWithEmptyFields["ff3"].Add(weathers.Count);
                            else if (propertiesWithEmptyFields["ff3"].Count > 0)
                                FixEmptyFields(propertiesWithEmptyFields, weathers, "ff3");

                            w.TotalCloudCover = row["N"].ToString();

                            //w.PresentWeather = row["WW"].ToString().Equals(string.Empty) ? prethodnaPresentWeather : row["WW"].ToString();
                            //prethodnaPresentWeather = w.PresentWeather;
                            //if (prethodnaPresentWeather.Equals(string.Empty))
                            //    propertiesWithEmptyFields["WW"].Add(weathers.Count);
                            //else if (propertiesWithEmptyFields["WW"].Count > 0)
                            //    FixEmptyFields(propertiesWithEmptyFields, weathers, "WW");

                            w.Visibility = row["VV"].ToString().Equals(string.Empty) ? prethodnaVisibility : row["VV"].ToString();
                            prethodnaVisibility = w.Visibility;

                            if (prethodnaVisibility.Equals(string.Empty))
                                propertiesWithEmptyFields["VV"].Add(weathers.Count);
                            else if (prethodnaVisibility.Contains("less"))
                            {
                                prethodnaVisibility = new string(prethodnaVisibility.ToCharArray().Where(c => Char.IsDigit(c) || Char.IsPunctuation(c)).ToArray());

                                prethodnaVisibility = (double.Parse(prethodnaVisibility) - (double.Parse(prethodnaVisibility) / 2)).ToString();

                                w.Visibility = prethodnaVisibility;

                                if (propertiesWithEmptyFields["VV"].Count > 0)
                                    FixEmptyFields(propertiesWithEmptyFields, weathers, "VV");
                            }
                            else if (propertiesWithEmptyFields["VV"].Count > 0)
                                FixEmptyFields(propertiesWithEmptyFields, weathers, "VV");

                            w.DewPointTemperature = row["Td"].ToString().Equals(string.Empty) ? prethondaDewPointTemperature : row["Td"].ToString();
                            prethondaDewPointTemperature = w.DewPointTemperature;
                            if (prethondaDewPointTemperature.Equals(string.Empty))
                                propertiesWithEmptyFields["Td"].Add(weathers.Count);
                            else if (propertiesWithEmptyFields["Td"].Count > 0)
                                FixEmptyFields(propertiesWithEmptyFields, weathers, "Td");

                            weathers.Add(w);

                            if (w.TotalCloudCover.Equals(string.Empty))
                            {
                                weathersWithEmptyFields.Add(w);
                            }
                            else
                            {
                                int value = -1;

                                if (w.TotalCloudCover.Contains("more") || w.TotalCloudCover.Contains("less"))
                                {
                                    double firstNumber = double.Parse(new string(w.TotalCloudCover.TakeWhile(char.IsDigit).ToArray()));
                                    double secondNumber = double.Parse(GetNumberFromEndOfString(w.TotalCloudCover));

                                    //90 or more, but not 100
                                    if (secondNumber > 0)
                                        value = new Random().Next((int)firstNumber, (int)secondNumber - 1);

                                    //10 or less, but not 0
                                    else
                                        value = new Random().Next((int)secondNumber + 1, (int)firstNumber);
                                }
                                //ove dve crtice su razlicite, ako se zagledas malo jedna je duza od druge :)
                                else if (Regex.IsMatch(w.TotalCloudCover, "[–]"))
                                {
                                    string[] parts = w.TotalCloudCover.Split('–');
                                    string firstNumber = parts[0].Trim(' ');
                                    string secondNumber = parts[1].Trim(' ');
                                    secondNumber = new string(secondNumber.TakeWhile(char.IsDigit).ToArray());
                                    value = new Random().Next(int.Parse(firstNumber), int.Parse(secondNumber));

                                }
                                else if (Regex.IsMatch(w.TotalCloudCover, "[-]"))
                                {
                                    string[] parts = w.TotalCloudCover.Split('-');
                                    string firstNumber = parts[0].Trim(' ');
                                    string secondNumber = parts[1].Trim(' ');
                                    secondNumber = new string(secondNumber.TakeWhile(char.IsDigit).ToArray());
                                    value = new Random().Next(int.Parse(firstNumber), int.Parse(secondNumber));

                                }
                                else if (w.TotalCloudCover.ToLower().Contains("sky"))
                                {
                                    //ovde ne znam sta da stavim, od magle se ne vide oblacii...
                                    value = 100;
                                }
                                else if (w.TotalCloudCover.ToLower().Contains("no"))
                                {
                                    //nema oblaka, valjda 0%
                                    value = 0;
                                }
                                else
                                {
                                    value = int.Parse(new string(w.TotalCloudCover.TakeWhile(char.IsDigit).ToArray()));
                                }

                                w.TotalCloudCover = value.ToString();

                                if (weathersWithEmptyFields.Count > 0)
                                    FillEmptyFields(weathersWithEmptyFields, "N", value);

                                //popunjeno polje jedno za drugim
                                else
                                    prethodnaVrednostZaCloudCoverage = value;


                            }
                        }

                        //za poslednji red u tabeli, ostace prazan ako nema ovoga
                        if (weathersWithEmptyFields.Count > 0)
                        {
                            if (prethodnaVrednostZaCloudCoverage + 2 <= 100)
                                weathersWithEmptyFields.ForEach(item => item.TotalCloudCover = (prethodnaVrednostZaCloudCoverage + 2).ToString());
                            else
                                weathersWithEmptyFields.ForEach(item => item.TotalCloudCover = (prethodnaVrednostZaCloudCoverage - 2).ToString());
                        }

                    }

                    if (sheet.TableName.Equals("load"))
                        foreach (DataRow row in sheet.Rows)
                        {
                            Load l = new Load();
                            l.Date = row["DateShort"].ToString().Split(' ')[0]; // mesec/dan/godina
                            l.FromTime = DateTime.Parse(row["TimeFrom"].ToString()).ToString("HH:mm");
                            l.ToTime = DateTime.Parse(row["TimeTo"].ToString()).ToString("HH:mm");

                            l.MWh = long.Parse(row["Load (MW/h)"].ToString());

                            potorsnja.Add(l);
                        }

                }

                List<WeatherAndLoad> finalData = new List<WeatherAndLoad>();

                foreach (var item in weathers)
                {
                    // ITEM DATE -> dan.mesec.godina sat:minut;
                    string[] parts = item.Date.Split(' ');
                    string time = parts[1];

                    string[] dateParts = parts[0].Split('.');
                    string validDate = dateParts[0] + "/" + dateParts[1] + "/" + dateParts[2];
                    // TIME -> weather( 00:00) ; load (0:00)
                    //if (parts[1].First().Equals('0'))
                    //    parts[1] = parts[1].Substring(1);

                    DateTime date = DateTime.ParseExact(validDate, "d/M/yyyy", CultureInfo.InvariantCulture);

                    string validDateFormat = string.Format("{0:M/d/yyyy}", date);

                    //item.MWh = potorsnja.First(x => x.Date.Equals(validDateFormat) && x.FromTime.Equals(time)).MWh;

                    WeatherAndLoad wl = new WeatherAndLoad()
                    {
                        Date = item.Date,
                        AirTemperature = double.Parse(item.AirTemperature),
                        AtmosphericPressure = double.Parse(item.AtmosphericPressure),
                        PressureTendency = double.Parse(item.PressureTendency),
                        RelativeHumidity = double.Parse(item.RelativeHumidity),
                        MeanWindSpeed = double.Parse(item.MeanWindSpeed),
                        MaxGustValue = double.Parse(item.MaxGustValue),
                        TotalCloudCover = double.Parse(item.TotalCloudCover),
                        Visibility = double.Parse(item.Visibility),
                        DewPointTemperature = double.Parse(item.DewPointTemperature),
                        MWh = potorsnja.First(x => x.Date.Equals(validDateFormat) && x.FromTime.Equals(time)).MWh,

                    };

                    finalData.Add(wl);
                    brojac = finalData.Count;
                }


                bool retval = await dataSloj.WriteToDataBase(finalData);


                return retval;
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return false;
            }
        }

        private void FixEmptyFields(Dictionary<string, List<int>> propertiesWithEmptyFields, List<Weather> weathers, string attribute)
        {
            switch (attribute.ToLower())
            {
                case "t":
                    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].AirTemperature = prethodnaAirTemperature);
                    break;
                case "po":
                    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].AtmosphericPressure = prethodnaAtmosphericPressure);
                    break;
                case "pa":
                    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].PressureTendency = prethodnaPressureTendency);
                    break;
                case "u":
                    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].RelativeHumidity = prethodnaRelativeHumidity);
                    break;
                case "ff":
                    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].MeanWindSpeed = prethodnaMeanWindSPeed);
                    break;
                case "ff3":
                    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].MaxGustValue = prethodnaMaxGustValue);
                    break;
                // reseno na drugi nacin, posto su procenti u pitanju i velika je ralika izmedju 2 poznata polja, 90 or more, a sledece 20-30%
                //case "n":
                //    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].AirTemperature = prethodnaAirTemperature);
                //    break;
                //ovo ispod nema potrebe 95 razlicitih polja ima ovaj atribut WW
                //case "ww":
                //    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].PresentWeather = prethodnaPresentWeather);
                //    break;
                case "vv":
                    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].Visibility = prethodnaVisibility);
                    break;
                case "td":
                    propertiesWithEmptyFields[attribute].ForEach(item => weathers[item].DewPointTemperature = prethondaDewPointTemperature);
                    break;

            }

            propertiesWithEmptyFields[attribute].Clear();

        }

        private void FillEmptyFields(List<Weather> lista, string atribut, int novaVrednost)
        {
            // total cloud coverage in excel file column 'N'
            if (atribut.Equals("N"))
            {

                if (prethodnaVrednostZaCloudCoverage == -1 || prethodnaVrednostZaCloudCoverage.Equals(novaVrednost))
                {
                    lista.ForEach(x => x.TotalCloudCover = novaVrednost.ToString());

                    prethodnaVrednostZaCloudCoverage = novaVrednost;
                }
                else
                {
                    int korak = ((Math.Abs(prethodnaVrednostZaCloudCoverage - novaVrednost)) / lista.Count);

                    //da u foreach-u ne smanjujem 'prethodna' jer je ona i u if uslovu
                    int temp = prethodnaVrednostZaCloudCoverage;

                    foreach (Weather weather in lista)
                    {
                        if (prethodnaVrednostZaCloudCoverage > novaVrednost)
                        {
                            if (korak == 0)
                                temp = new Random().Next(novaVrednost, prethodnaVrednostZaCloudCoverage);
                            else
                                temp -= korak;
                        }
                        else
                        {
                            if (korak == 0)
                                temp = new Random().Next(prethodnaVrednostZaCloudCoverage, novaVrednost);
                            else
                                temp += korak;
                        }

                        weather.TotalCloudCover = temp.ToString();
                    }

                    prethodnaVrednostZaCloudCoverage = novaVrednost;

                }


            }
            else if (atribut.Equals(""))
            {

            }

            lista.Clear();
        }

        private string GetNumberFromEndOfString(string input)
        {
            var stack = new Stack<char>();

            for (var i = input.Length - 1; i >= 0; i--)
            {
                if (input[i].Equals('%') || input[i].Equals('.'))
                    continue;

                else if (!char.IsNumber(input[i]))
                {
                    break;
                }

                stack.Push(input[i]);
            }

            var result = new string(stack.ToArray());

            return result;
        }

        public async Task<bool> Training()
        {

            IDatabase dataSloj = new DatabaseService();
            List<WeatherAndLoad> data = await dataSloj.LoadFromDataBase();

            var minAirTemperature = data.Min(x => x.AirTemperature);
            var maxAirTemperature = data.Max(x => x.AirTemperature);

            var minHumidity = data.Min(x => x.RelativeHumidity);
            var maxHumidity = data.Max(x => x.RelativeHumidity);

            var minAtmosphericPressure = data.Min(x => x.AtmosphericPressure);
            var maxAtmosphericPressure = data.Max(x => x.AtmosphericPressure);

            var minPressureTendency = data.Min(x => x.PressureTendency);
            var maxPressureTendency = data.Max(x => x.PressureTendency);

            var minMeanWindSpeed = data.Min(x => x.MeanWindSpeed);
            var maxMeanWindSpeed = data.Max(x => x.MeanWindSpeed);

            var minMaxGustValue = data.Min(x => x.MaxGustValue);
            var maxMaxGustValue = data.Max(x => x.MaxGustValue);

            var minTotalCloudCover = data.Min(x => x.TotalCloudCover);
            var maxTotalCloudCover = data.Max(x => x.TotalCloudCover);

            var minVisibility = data.Min(x => x.Visibility);
            var maxVisibility = data.Max(x => x.Visibility);

            var minDewPointTemperature = data.Min(x => x.DewPointTemperature);
            var maxDewPointTemperature = data.Max(x => x.DewPointTemperature);

            var minMWh = data.Min(x => x.MWh);
            var maxMWh = data.Max(x => x.MWh);

            return true;

        }

    }
}
