
using ppee_dataLayer.Entities;
using ppee_dataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Services
{
    public class DatabaseService : IDatabase
    {
        public async Task<bool> WriteToDataBase(List<Load> potorsnja)
        {
            try
            {
                using (var db = new PPEE_DataContext())
                {
                    foreach (Load l in potorsnja)
                    {
                        await db.Loads.AddAsync(l);
                    }

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
