using System;

using Microsoft.AspNetCore.Mvc;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.Web;
using DwFramework.Web.Plugins;

namespace _Test.Web
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpPost("t")]
        public IActionResult Test(Body body)
        {
            ResultInfo result = null;
            try
            {
                var jwt = JwtManager.DecodeToken(body.Jwt);
                result = ResultInfo.Success("ok");
            }
            catch (Exception ex)
            {
                result = ResultInfo.Fail(ex.Message);
            }
            return Ok(result);
        }
    }


    public class Body
    {
        public string Jwt { get; set; }
    }
}
