using ExcelDataReader;
using Newtonsoft.Json;
using Numpy;
using ppee_dataLayer.Entities;
using ppee_dataLayer.Interfaces;
using ppee_dataLayer.Services;
using ppee_service.Interfaces;
using ppee_service.MyKerasEntity;
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

                //Svaki ucitan fajl je dodavanje u bazu,
                //Dakle ovde ucitaj iz baze ono sto vec ima
                // provera da li postoji je po atributu finalData.exist(x=> x.date.equals(item.date));
                List<WeatherAndLoad> finalData = await dataSloj.LoadFromDataBase();
                int countFromDataBase = finalData.Count;
                MinMaxValues minMaxValues = await dataSloj.LoadMinMaxValues();

                foreach (var item in weathers)
                {
                    //ako vec postoji u bazi, preskoci ga
                    // postoji ako su isti datum i vreme
                    if (finalData.Exists(x => x.Date.Equals(item.Date.Replace('.', '/'))))
                        continue;

                    // ITEM DATE -> dan.mesec.godina sat:minut;
                    string[] parts = item.Date.Split(' ');
                    string time = parts[1];

                    //string[] dateParts = parts[0].Split('.');
                    //string validDate = dateParts[0] + "/" + dateParts[1] + "/" + dateParts[2];

                    string validDate = parts[0].Replace('.', '/'); //samo datum

                    // TIME -> weather( 00:00) ; load (0:00)
                    //if (parts[1].First().Equals('0'))
                    //    parts[1] = parts[1].Substring(1);

                    DateTime date = DateTime.ParseExact(validDate, "d/M/yyyy", CultureInfo.InvariantCulture);

                    string validDateFormat = string.Format("{0:M/d/yyyy}", date);

                    //item.MWh = potorsnja.First(x => x.Date.Equals(validDateFormat) && x.FromTime.Equals(time)).MWh;

                    WeatherAndLoad wl = new WeatherAndLoad()
                    {
                        Date = item.Date.Replace('.', '/'), //ovde ide datum i vreme
                        DayOfWeek = (float)(DateTime.Parse(validDateFormat).DayOfWeek),
                        Hour = float.Parse(parts[1].Split(':')[0]),
                        Month = DateTime.Parse(validDateFormat).Month,
                        AirTemperature = float.Parse(item.AirTemperature),
                        AtmosphericPressure = float.Parse(item.AtmosphericPressure),
                        PressureTendency = float.Parse(item.PressureTendency),
                        RelativeHumidity = float.Parse(item.RelativeHumidity),
                        MeanWindSpeed = float.Parse(item.MeanWindSpeed),
                        MaxGustValue = float.Parse(item.MaxGustValue),
                        TotalCloudCover = float.Parse(item.TotalCloudCover),
                        Visibility = float.Parse(item.Visibility),
                        DewPointTemperature = float.Parse(item.DewPointTemperature),
                        //MWh = potorsnja.First(x => x.Date.Equals(validDateFormat) && x.FromTime.Equals(time)).MWh,

                    };

                    //ako ima load u excel fajlu
                    if (potorsnja.Count > 0)
                    {
                        try
                        {
                            //nadji ga na osnovu datuma
                            wl.MWh = potorsnja.First(x => x.Date.Equals(validDateFormat) && x.FromTime.Equals(time)).MWh;
                        }
                        catch (Exception ex)
                        {
                            string a = ex.Message;

                            wl.MWh = minMaxValues.MinMWh;
                        }

                    }
                    else
                    {
                        wl.MWh = minMaxValues.MinMWh;
                    }

                    finalData.Add(wl);
                    brojac = finalData.Count;
                }

                bool retval = await dataSloj.WriteToDataBase(finalData.Skip(countFromDataBase).ToList());
                bool min_max_retval = await dataSloj.UpdateMinMaxValues(KerasHelpers.FindMinAndMaxValues(finalData));

                return retval && min_max_retval ? true : false;

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

        public async Task<bool> Training(string startDate, string endDate)
        {
            try
            {
                DateTime start = DateTime.ParseExact(startDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(endDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                end = end.AddHours(23); //kraj dana :)                

                if (!ValidationForTrainingAndPrediction(start, end)) return false;

                IDatabase dataSloj = new DatabaseService();
                List<WeatherAndLoad> data = await dataSloj.LoadFromDataBase();
                MinMaxValues minMaxValues = await dataSloj.LoadMinMaxValues();

                //testiranje treninga za razne modele 
                //data = data.Take(3000).ToList();
                //end testiranje


                // UZETI PODATKE U DATUMSKOM OPSEGU

                data = await GetWeatherLoadDataInDateRange(data, start, end);

                //END PODACI U DATUMSKOM OPSEGU


                List<List<float>> predictorData = KerasHelpers.ScaleDataSetAndGetPredictorData(data, minMaxValues);
                //moze ovako jer u metodi scale skaliram i MWh u opsegu 0-1
                float[] predictedData = data.Select(x => x.MWh).ToArray();

                if (predictorData.Count != predictedData.Length)
                    return false;

                //svaki trening uzimam 95 trening, 5 posto test da bih izracunao gresku i da sacuvam model kao
                // model_{mape_error}
                const float frac = 0.95f;
                int trainCount = (int)Math.Round((data.Count * frac), 0);

                var predictorTraining = predictorData.Take(trainCount).ToList();
                var predictorTest = predictorData.Skip(trainCount).ToList();

                float[] predictedTraining = predictedData.Take(trainCount).ToArray();
                List<float> predictedTest = predictedData.Skip(trainCount).ToList();

                float[,] matrixPredictorData = KerasHelpers.CreateMatrix(predictorTraining, predictorTraining.Count, predictorTraining[0].Count);
                var NdPredictorData = np.array(matrixPredictorData);
                var NdPredictedData = np.array(predictedTraining);

                int inputDim = predictorTraining[0].Count;
                MyKeras myKeras = new MyKeras(inputDim);

                var model = myKeras.TrainModel(NdPredictorData, NdPredictedData);


                //predikicija zbog cuvanja modela sa apsolutno greskom
                float[,] matrixPredictorTest = KerasHelpers.CreateMatrix(predictorTest, predictorTest.Count, predictorTest[0].Count);
                var NdPredictorTest = np.array(matrixPredictorTest).astype(np.float32);

                var predictionValues = model.Predict(NdPredictorTest);

                var results = KerasHelpers.NDarrayToList(predictionValues);

                predictedTest = KerasHelpers.InverseTransform_MWh(predictedTest, minMaxValues);
                results = KerasHelpers.InverseTransform_MWh(results, minMaxValues);

                double squareError = KerasHelpers.GetSquareDeviation(results, predictedTest);
                double absoluteError = KerasHelpers.GetAbsoluteDeviation(results, predictedTest);

                //cuvanje treniranog modela -> model_mapeError.( json | h5)
                myKeras.SaveModel(model, Math.Round(absoluteError, 2).ToString());

                return true;
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                throw ex;
            }
        }

        public async Task<Tuple<string, double>> Predict(string startDate, int numberOfDays)
        {

            try
            {
                DateTime start = DateTime.ParseExact(startDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime end = start.AddDays(numberOfDays).AddHours(23);

                if (!ValidationForTrainingAndPrediction(start, end, true)) return null;

                IDatabase dataSloj = new DatabaseService();
                List<WeatherAndLoad> data = await dataSloj.LoadFromDataBase();
                MinMaxValues minMaxValues = await dataSloj.LoadMinMaxValues();

                // UZETI PODATKE U DATUMSKOM OPSEGU

                data = await GetWeatherLoadDataInDateRange(data, start, end);

                //END PODACI U DATUMSKOM OPSEGU

                List<List<float>> predictorData = KerasHelpers.ScaleDataSetAndGetPredictorData(data, minMaxValues);
                //moze ovako jer u metodi scale skaliram i MWh u opsegu 0-1
                float[] predictedData = data.Select(x => x.MWh).ToArray();

                if (predictorData.Count != predictedData.Length)
                    return null;

                //Kada je radjeno 90% trening 10% test nad svim podacima

                //const float frac = 0.9f;
                //int trainCount = (int)Math.Round((data.Count * frac), 0);

                //ovo je ulaz u model, nezavisni podaci, temp, cloud, humidity...
                //var predictorTest = predictorData.Skip(trainCount).ToList();

                //ovo je ono sto se ocekuje na izlazu, MWh
                //List<float> predictedTest = predictedData.Skip(trainCount).ToList();
                //float[,] matrixPredictorTest = KerasHelpers.CreateMatrix(predictorTest, predictorTest.Count, predictorTest[0].Count);

                //end 


                float[,] matrixPredictorTest = KerasHelpers.CreateMatrix(predictorData, predictorData.Count, predictorData[0].Count);
                var NdPredictorTest = np.array(matrixPredictorTest).astype(np.float32);


                int inputDim = predictorData[0].Count;
                MyKeras myKeras = new MyKeras(inputDim);

                var results = myKeras.Predict(NdPredictorTest);

                if (results == null) return null;

                results = KerasHelpers.InverseTransform_MWh(results, minMaxValues);

                //moje racunanje greske

                //predictedTest = KerasHelpers.InverseTransform_MWh(predictedTest, minMaxValues);

                //double squareError = KerasHelpers.GetSquareDeviation(results, predictedTest);
                //double absoluteError = KerasHelpers.GetAbsoluteDeviation(results, predictedTest);

                //end moje racunanje greske

                // Mapiranje podataka za export u csv, upis u bazu i vracanje klijentu
                //List<WeatherAndLoad> weahterWithRealDate = data.Skip(trainCount).ToList();
                List<WeatherAndLoad> weahterWithRealDate = data;
                List<ForecastValues> dataForCsv = new List<ForecastValues>();

                for (int i = 0; i < results.Count; i++)
                {
                    ForecastValues f = new ForecastValues()
                    {
                        DateAndTime = weahterWithRealDate[i].Date,
                        Load = Math.Round(results[i], 2),
                    };

                    dataForCsv.Add(f);
                }

                await dataSloj.WritePredictedValues(dataForCsv);
                await ExportToCSV(dataForCsv);

                string jsonData = JsonConvert.SerializeObject(dataForCsv);
                //Tuple<string, double> retVal = new Tuple<string, double>(jsonData, absoluteError);
                Tuple<string, double> retVal = new Tuple<string, double>(jsonData, 0);

                return retVal;
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                throw ex;
            }
        }

        public Task<bool> ExportToCSV(List<ForecastValues> data, string dateRange = null)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                char[] charsToTrim = { '\\', ' ' };
                path = path.TrimEnd(charsToTrim);
                path = path.Substring(0, path.LastIndexOf('\\'));

                if (dateRange != null)
                {
                    path += "\\DateRangeCSV\\";
                    path += "Report_" + dateRange;
                }
                else
                {
                    path += "\\EveryPredictionCSV\\";
                    string date = DateTime.Now.ToString("dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture);
                    date = date.Replace('/', '-').Replace(':', '-').Replace(' ', '_');

                    path += "Prediction_" + date;
                }

                path += ".csv";

                var csv = new StringBuilder();

                csv.AppendLine("Date and Time , MWh");

                data.ForEach(item => csv.AppendLine($"{item.DateAndTime} , {item.Load}"));

                File.WriteAllText(path, csv.ToString());

                return Task.FromResult<bool>(true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetForecastValues(string startDate, string endDate, bool exportToCSV = false)
        {
            try
            {
                DateTime start = DateTime.ParseExact(startDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(endDate, "d/M/yyyy", CultureInfo.InvariantCulture);

                end = end.AddHours(23); //kraj dana :)

                if (!ValidationForTrainingAndPrediction(start, end, true)) return "-1";

                IDatabase dataSloj = new DatabaseService();
                List<ForecastValues> data = await dataSloj.LoadPredictedValues();

                var retVal = await FindDataInDateRange(data, start, end);

                if (exportToCSV && retVal.Count > 0)
                {
                    startDate = startDate.Replace('/', '-');
                    endDate = endDate.Replace('/', '-');
                    string dateRange = startDate + "_" + endDate;

                    await ExportToCSV(retVal, dateRange);

                    return "Data Exported successfully";
                }

                return JsonConvert.SerializeObject(retVal);
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                throw ex;
            }
        }

        private Task<List<ForecastValues>> FindDataInDateRange(List<ForecastValues> data, DateTime start, DateTime end)
        {
            List<ForecastValues> retVal = new List<ForecastValues>();

            foreach (var item in data)
            {
                var temp = DateTime.ParseExact(item.DateAndTime, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);

                if (temp >= start && temp <= end)
                    retVal.Add(item);
            }

            return Task.FromResult<List<ForecastValues>>(retVal);
        }

        private Task<List<WeatherAndLoad>> GetWeatherLoadDataInDateRange(List<WeatherAndLoad> data, DateTime start, DateTime end)
        {
            List<WeatherAndLoad> retVal = new List<WeatherAndLoad>();

            foreach (var item in data)
            {
                var temp = DateTime.ParseExact(item.Date, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);

                if (temp >= start && temp <= end)
                    retVal.Add(item);
            }

            return Task.FromResult<List<WeatherAndLoad>>(retVal);
        }

        private bool ValidationForTrainingAndPrediction(DateTime start, DateTime end, bool predictionOrReport = false)
        {
            DateTime allDataStart = DateTime.ParseExact("01/01/2016 00:00", "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);
            DateTime allDataEnd;

            if (predictionOrReport)
                allDataEnd = DateTime.ParseExact("31/05/2019 23:00", "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);
            else
                allDataEnd = DateTime.ParseExact("05/05/2019 23:00", "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);

            if (start < allDataStart || start > allDataEnd)
                return false;

            if (end < allDataStart || end > allDataEnd)
                return false;

            return true;
        }

        public async Task<string> AddPowerPlant(dynamic data)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(data);
                PowerPlant powerPlant = JsonConvert.DeserializeObject<PowerPlant>(jsonString);

                string validation = ValidationForPowerPlant(powerPlant);

                if (validation != string.Empty) return validation;

                if (powerPlant.Type.ToLower().Equals("solar"))
                {
                    int Imax = 1000;
                    powerPlant.MinimumOutputPower = 0;
                    powerPlant.MaximumOutputPower = Imax * powerPlant.Efficiency * powerPlant.SurfaceArea;

                }
                else if (powerPlant.Type.ToLower().Equals("wind"))
                {
                    powerPlant.MinimumOutputPower = 0;
                    powerPlant.MaximumOutputPower = (int)(Math.Pow((powerPlant.BladesSweptAreaDiameter / 2), 2) * Math.PI);
                }
                else if (powerPlant.Type.ToLower().Equals("hydro"))
                {
                    powerPlant.MinimumOutputPower = 0;
                }

                IDatabase dataSloj = new DatabaseService();
                if (await dataSloj.AddPowerPlant(powerPlant))
                    return string.Empty;

                return "Some error ocured";
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return "Invalid data";
            }

        }

        public async Task<bool> DeletePowerPlant(int id)
        {
            IDatabase dataSloj = new DatabaseService();
            return await dataSloj.DeletePowerPlant(id);
        }

        public async Task<string> GetAllPowerPlants()
        {
            IDatabase dataSloj = new DatabaseService();
            var result = await dataSloj.GetAllPowerPlants();
            if (result.Count > 0)
                return JsonConvert.SerializeObject(result);
            else
                return "-1";
        }

        public Task<string> GetDefaultOptimizationSettings()
        {
            OptimizationSettings optimizationSettings = new OptimizationSettings();
            optimizationSettings.CO2Coal = 200; //Pounds of CO2 emitted per million British thermal units (Btu) of energy for various fuels:
            optimizationSettings.CO2Gas = 50;  // Pounds of CO2 emitted per million British thermal units (Btu) of energy for various fuels:
            optimizationSettings.CostGas = 300; // natural gas combined cycle coming in at $41 to $74 per MWh
            optimizationSettings.CostCoal = 100; //coal cost between $57 and $148 per megawatt-hour.
            optimizationSettings.Date = "";
            optimizationSettings.OptimizationType = "cost";
            optimizationSettings.PowerPlantsForOptimization = new List<PowerPlant>();
            optimizationSettings.WeightFactor = 0;
            optimizationSettings.PowerPlantAndNumberForOptimization = new List<IdAndNumber>();

            return Task.FromResult(JsonConvert.SerializeObject(optimizationSettings));
        }

        public async Task<string> OptimizationForDay(dynamic optimizationSettingsDynamic)
        {
            try
            {
                string optimizationSettingsJSON = JsonConvert.SerializeObject(optimizationSettingsDynamic);

                OptimizationSettings optimizationSettings = JsonConvert.DeserializeObject<OptimizationSettings>(optimizationSettingsJSON);


                string validation = ValidationForOptimizationSettings(optimizationSettings);

                if (validation != string.Empty)
                {
                    if (validation == "-1") return "-1";

                    return "validation:" + validation;
                }

                IDatabase dataBase = new DatabaseService();

                List<PowerPlant> powerPlants = await dataBase.GetAllPowerPlants();

                optimizationSettings.PowerPlantsForOptimization = new List<PowerPlant>();

                foreach (var item in optimizationSettings.PowerPlantAndNumberForOptimization)
                {
                    var powerPlant = powerPlants.Find(x => x.Id == item.Id);
                    if (powerPlant != null)
                    {
                        for (int i = 0; i < item.Number; i++)
                        {
                            optimizationSettings.PowerPlantsForOptimization.Add(powerPlant);
                        }
                    }
                }

                Optimization.Optimization optimization = new Optimization.Optimization();
                List<OptimizationPerHour> dayOptimization = await optimization.CreateOptimization(optimizationSettings);

                if (dayOptimization == null)
                    return "-1";

                if (dayOptimization.Count == 0)
                    return "Try with more generators";

                if (dayOptimization.Count == 1)
                    return "Sorry. There is no production prediction for choosen date";

                return JsonConvert.SerializeObject(dayOptimization);
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return "-1";
            }

        }

        private string ValidationForOptimizationSettings(OptimizationSettings settings)
        {
            try
            {
                DateTime date;
                DateTime.TryParseExact(settings.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                //nije uspeo da pretvori datum
                if (date == DateTime.MinValue)
                    return "Please provide valid date";

                DateTime allDataStart = DateTime.ParseExact("04/01/2019", "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime allDataEnd = DateTime.ParseExact("31/05/2019", "d/M/yyyy", CultureInfo.InvariantCulture);

                if (date < allDataStart || date > allDataEnd)
                    return "Wrong date. Choose date from 04/01/2019 to 31/05/2019";

                if (settings.CostCoal < 1)
                    return "Value for coal cost must be greater than 1";

                if (settings.CostGas < 1)
                    return "Value for gas cost must be greater than 1";

                if (settings.CO2Coal < 1)
                    return "Value for coal CO2 emission must be greater than 1";

                if (settings.CO2Gas < 1)
                    return "Value for gas CO2 emission must be greater than 1";

                if (string.IsNullOrEmpty(settings.OptimizationType) || string.IsNullOrWhiteSpace(settings.OptimizationType))
                    return "Please select type of optimization";

                if (settings.WeightFactor < 0 || settings.WeightFactor > 1)
                    return "Weight factor must be between 0 and 1";

                return string.Empty;
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return "-1";
            }
        }

        private string ValidationForPowerPlant(PowerPlant powerPlant)
        {
            string str = string.Empty;
            try
            {
                double name = double.Parse(powerPlant.Name);
                return "Name cannot be only numbers. Please provide at least one alphabet character";
            }
            catch (Exception ex)
            {
                string a = ex.Message;
            }

            if (string.IsNullOrEmpty(powerPlant.Name) || string.IsNullOrWhiteSpace(powerPlant.Name))
                return "Please provide name for generator";

            if (powerPlant.Type.ToLower().Equals("solar"))
            {
                if (powerPlant.Efficiency < 1)
                    str = "Efficiency must be greater than 1";
                if (powerPlant.SurfaceArea < 1)
                    str = "Surface area must be greater than 1";
            }
            else if (powerPlant.Type.ToLower().Equals("wind"))
            {
                if (powerPlant.BladesSweptAreaDiameter < 20)
                    str = "Blades swept area diameter must be greater than 20";
                if (powerPlant.NumberOfWindGenerators < 1)
                    str = "Number of wind generatros must be greater than 1";
            }
            else
            {
                if (powerPlant.MaximumOutputPower < 1)
                    str = "Maximum power must be greater than 1";

                if (powerPlant.Type.ToLower().Equals("hydro") == false)
                {
                    //coal or gas
                    if (powerPlant.MinimumOutputPower < 1)
                        str = "Minimum output power must be greater than 1";
                }

                if (powerPlant.MinimumOutputPower > powerPlant.MaximumOutputPower)
                    str = "Minimum power cannot be greater than maximum power";

            }

            return str;
        }
    }
}
