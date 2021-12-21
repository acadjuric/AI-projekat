using ppee_dataLayer.Entities;
using ppee_dataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Services
{
    public class DatabaseService : IDatabase
    {
        public async Task<bool> WriteToDataBase(List<WeatherAndLoad> finalData)
        {
            try
            {
                using (var db = new PPEE_DataContext())
                {
                   
                    db.WeatherAndLoads.AddRange(finalData);

                    await db.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return false;
            }
        }
    }
}
