using ppee_dataLayer.Entities;
using ppee_dataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Services
{
    public class DatabaseService : IDatabase
    {
        public async Task<List<WeatherAndLoad>> LoadFromDataBase()
        {
            try
            {
                using(var db = new PPEE_DataContext())
                {
                    
                    return await db.WeatherAndLoads.ToListAsync();
                }
            }
            catch(Exception ex)
            {
                string a = ex.Message;
                return null;
            }
        }

        public async Task<MinMaxValues> LoadMinMaxValues()
        {
            try
            {
                using(var db = new PPEE_DataContext())
                {
                    return await db.MinMaxValues.FirstAsync();
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateMinMaxValues(MinMaxValues value)
        {
            try
            {
                using(var db = new PPEE_DataContext())
                {
                    if(db.MinMaxValues.ToList().Count > 0)
                        db.MinMaxValues.Remove(await db.MinMaxValues.FirstAsync());

                    db.MinMaxValues.Add(value);

                    await db.SaveChangesAsync();

                    return true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

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
