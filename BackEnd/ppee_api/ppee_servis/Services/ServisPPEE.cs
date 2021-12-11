using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using ppee_dataLayer.Entities;
using ppee_dataLayer.Interfaces;
using ppee_dataLayer.Services;
using ppee_servis.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_servis.Services
{
    public class ServisPPEE : IServisPPEE
    {

        private IDatabase dataSloj = new DatabaseService();

        public async Task<bool> ReadFile(Stream stream)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                });

                var sheets = result.Tables;
                List<Load> potorsnja = new List<Load>();
                foreach (DataTable sheet in sheets)
                {
                    if(sheet.TableName.Equals("load"))
                        foreach(DataRow row in sheet.Rows)
                        {
                            Load l = new Load();
                            l.Date = row["DateShort"].ToString();
                            l.FromTime = row["TimeFrom"].ToString();
                            l.ToTime = row["TimeTo"].ToString();
                            l.MWh = long.Parse(row["Load (MW/h)"].ToString());

                            potorsnja.Add(l);
                        }

                }

                bool retval = await dataSloj.WriteToDataBase(potorsnja);

                return retval;
            }
            catch(Exception ex)
            {
                string a = ex.Message;
                return false;
            }
        }
    }
}
