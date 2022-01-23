using ppee_dataLayer.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_service.Interfaces
{
    public interface IServicePPEE
    {
        Task<bool> ReadFile(Stream stream);

        Task<bool> Training(string startDate, string endDate);

        Task<Tuple<string, double>> Predict(string startDate, int numberOfDays);

        //Task<bool> ExportToCSV(List<ForecastValues> data, bool dataRange = false);

        Task<string> GetForecastValues(string startDate, string endDate, bool exportToCSV = false);

        Task<bool> AddPowerPlant(dynamic data);

        Task<bool> DeletePowerPlant(int id);

        Task<string> GetAllPowerPlants();

        Task<string> GetDefaultOptimizationSettings();

        Task<string> Optimization(dynamic optimizationSettingsDynamic);

    }
}
