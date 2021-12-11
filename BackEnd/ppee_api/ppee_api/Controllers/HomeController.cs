using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ppee_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class HomeController : ControllerBase
    {
        public HomeController()
        {

        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("FileUpload")]
        public async Task<IActionResult> FileUpload()
        {
            try
            {
                var file = Request.Form.Files[0];
                string a = file.Name;

                if(file.Length > 0)
                {
                    return Ok();
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