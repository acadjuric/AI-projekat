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
        public async Task<bool> AddPowerPlant(PowerPlant powerPlant)
        {
            try
            {
                using (var db = new PPEE_DataContext())
                {
                    var old = (await db.PowerPlants.ToListAsync()).First(item => item.Name.ToLower().Equals(powerPlant.Name.ToLower()) && item.Type.ToLower().Equals(powerPlant.Type.ToLower()));
                    if (old == null)
                    {
                        db.PowerPlants.Add(powerPlant);
                        await db.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return false;
            }
        }

        public async Task<bool> DeletePowerPlant(int id)
        {
            try
            {
                using (var db = new PPEE_DataContext())
                {
                    var item = (await db.PowerPlants.ToListAsync()).First(x => x.Id.Equals(id));
                    if (item == null)
                        return false;

                    db.PowerPlants.Remove(item);
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

        public async Task<List<WeatherAndLoad>> LoadFromDataBase()
        {
            try
            {
                using (var db = new PPEE_DataContext())
                {

                    return await db.WeatherAndLoads.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return null;
            }
        }

        public async Task<MinMaxValues> LoadMinMaxValues()
        {
            try
            {
                using (var db = new PPEE_DataContext())
                {
                    return await db.MinMaxValues.FirstAsync();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ForecastValues>> LoadPredictedValues()
        {
            try
            {
                using (var db = new PPEE_DataContext())
                {
                    return await db.ForecastValues.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateMinMaxValues(MinMaxValues value)
        {
            try
            {
                using (var db = new PPEE_DataContext())
                {
                    if (db.MinMaxValues.ToList().Count > 0)
                        db.MinMaxValues.Remove(await db.MinMaxValues.FirstAsync());

                    db.MinMaxValues.Add(value);

                    await db.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> WritePredictedValues(List<ForecastValues> data)
        {
            try
            {
                using (var db = new PPEE_DataContext())
                {
                    var dataFromDataBase = await db.ForecastValues.ToListAsync();

                    foreach (var item in data)
                    {
                        var oldItem = dataFromDataBase.Find(old => old.DateAndTime.Equals(item.DateAndTime));
                        if (oldItem != null)
                        {
                            db.ForecastValues.Remove(oldItem);
                        }

                        db.ForecastValues.Add(item);
                    }

                    await db.SaveChangesAsync();

                }

                return true;
            }
            catch (Exception ex)
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
