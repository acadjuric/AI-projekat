using ppee_dataLayer.Entities;
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
    }
}
