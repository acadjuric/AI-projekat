using ppee_api.Models;
using ppee_service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ppee_api.Controllers
{
    public class HomeController : ApiController
    {
        IServicePPEE service;
        public HomeController(IServicePPEE service)
        {
            this.service = service;
        }

        [HttpPost, Route("api/home/training")]
        public async Task<HttpResponseMessage> TrainModel(Training model)
        {
            if (await service.Training(model.FromDate, model.ToDate))
                return Request.CreateResponse<string>(HttpStatusCode.OK, "Training successfully completed");

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Some error ocured");

        }

        [HttpPost, Route("api/home/forecast")]
        public async Task<HttpResponseMessage> Forecast(Prediction model)
        {
            Tuple<string, double> retVal = await service.Predict(model.StartDate, model.NumberOfDays);

            if (retVal != null)
                return Request.CreateResponse<Tuple<string, double>>(HttpStatusCode.OK, retVal);

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Some error ocured");
        }

        [HttpPost, Route("api/home/report")]
        public async Task<HttpResponseMessage> GetForcastValues(Training model)
        {
            //Training model korisim jer ocekujem iste atribute kao kod treninga ( od datuma - do datuma)
            //var a = await service.GetForecastValues(model.FromDate, model.ToDate);

            string jsonData = await service.GetForecastValues(model.FromDate, model.ToDate);
            return Request.CreateResponse<string>(HttpStatusCode.OK, jsonData);

        }

        [HttpPost, Route("api/home/export")]
        public async Task<HttpResponseMessage> ExportReportToCSV(Training model)
        {
            string response = await service.GetForecastValues(model.FromDate, model.ToDate, exportToCSV: true);

            return Request.CreateResponse<string>(HttpStatusCode.OK, response);
        }

        [HttpPost, Route("api/home/fileupload")]
        public async Task<HttpResponseMessage> FileUpload()
        {
            try
            {

                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "UnsupportedMediaType");
                }

                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();

                foreach (var stream in filesReadToProvider.Contents)
                {
                    var filestream = await stream.ReadAsStreamAsync();
                    if (filestream.Length > 0)
                    {
                        if (await service.ReadFile(filestream))
                        {
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File error");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File error");


                    }
                }

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File error");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, $"{ex}");
            }
        }

        [HttpPost, Route("api/home/addpowerplant")]
        public async Task<HttpResponseMessage> AddPowerPlant(dynamic jsonData)
        {
            try
            {
                 bool retVal = await service.AddPowerPlant(jsonData);
                if (retVal)
                   return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,"Invalid data");
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, $"{ex}");
            }
        }

        [HttpDelete, Route("api/home/deletepowerplant/{id}")]
        public async Task<HttpResponseMessage> DeletePowerPlant(int id)
        {
            try
            {
                bool retVal = await service.DeletePowerPlant(id);
                if (retVal)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid id");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, $"{ex}");
            }
        }

        [HttpGet, Route("api/home/getpowerplant")]
        public async Task<HttpResponseMessage> GetAllPowerPlants()
        {
            try
            {
                var retVal = await service.GetAllPowerPlants();
                if (retVal != "-1")
                    return Request.CreateResponse<string>(HttpStatusCode.OK, retVal);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No data");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, $"{ex}");
            }
        }

        [HttpGet, Route("api/home/optimizationsettings")]
        public async Task<HttpResponseMessage> GetDeafultOptimizationSettings()
        {
            try
            {
                string settingsJSON = await service.GetDefaultOptimizationSettings();
                    return Request.CreateResponse<string>(HttpStatusCode.OK, settingsJSON);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, $"{ex}");
            }
        }

        [HttpPost, Route("api/home/optimize")]
        public async Task<HttpResponseMessage> Optimize(dynamic optimizationSettings)
        {
            try
            {
                string settingsJSON = await service.Optimization(optimizationSettings);

                return Request.CreateResponse<string>(HttpStatusCode.OK, settingsJSON);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, $"{ex}");
            }
        }
    }
}
