using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ppee_servis.Interfaces;

namespace ppee_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class HomeController : ControllerBase
    {
        IServisPPEE servis;

        public HomeController(IServisPPEE servis)
        {
            this.servis = servis;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("FileUpload")]
        public async Task<IActionResult> FileUpload()
        {
            try
            {
                if (Request.Form.Files.Count != 0) {
                    var file = Request.Form.Files[0];
                    string a = file.Name;

                    if (file.Length > 0)
                    {
                        var stream = file.OpenReadStream();

                        if (await servis.ReadFile(stream))
                        {
                            return Ok();
                        }
                        else
                            return BadRequest();

                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }

            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error {ex}");
            }
        }


    }
}