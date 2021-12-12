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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_service.Services
{
    public class ServicePPEE : IServicePPEE
    {
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
                foreach (DataTable sheet in sheets)
                {
                    if (sheet.TableName.Equals("load"))
                        foreach (DataRow row in sheet.Rows)
                        {
                            if (potorsnja.Count == 1000) break;

                            Load l = new Load();
                            l.Date = row["DateShort"].ToString();
                            l.FromTime = row["TimeFrom"].ToString();
                            l.ToTime = row["TimeTo"].ToString();
                            l.MWh = long.Parse(row["Load (MW/h)"].ToString());

                            potorsnja.Add(l);
                        }

                }

                bool retval = await dataSloj.WriteToDataBase(potorsnja);

                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                engine.ExecuteFile(@"C:\Users\Asus\Desktop\AI-projekat\API\ppee_api\ppee_service\Python\skripta.py", scope);
                dynamic prvaFunkcija = scope.GetVariable("prvaFunkcija");
                List<Load> aca = prvaFunkcija(potorsnja);

                return aca.Count == 30? true:false;
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return false;
            }
        }
    }
}
