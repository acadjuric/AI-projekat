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

        [HttpPost,Route("api/home/training")]
        public async Task<HttpResponseMessage> TrainModel()
        {
            if (await service.Training())
                return Request.CreateResponse(HttpStatusCode.OK);

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Some error ocured :)");
            
        }

        [HttpPost,Route("api/home/fileupload")]
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
    }
}
