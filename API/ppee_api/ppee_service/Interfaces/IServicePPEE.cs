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

        Task<Tuple<double, double>> Predict(string startDate, int numberOfDays);
    }
}
