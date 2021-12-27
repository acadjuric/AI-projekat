﻿using ppee_dataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Interfaces
{
    public interface IDatabase
    {
        Task<bool> WriteToDataBase(List<WeatherAndLoad> finalData);

        Task<List<WeatherAndLoad>> LoadFromDataBase();

        Task<MinMaxValues> LoadMinMaxValues();

        Task<bool> UpdateMinMaxValues(MinMaxValues value);

        Task<bool> WritePredictedValues(List<ForecastValues> data);

        Task<List<ForecastValues>> LoadPredictedValues(DateTime startDate, DateTime endDate);
    }
}
