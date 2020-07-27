using System;

using Microsoft.AspNetCore.Mvc;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.WebAPI;
using DwFramework.WebAPI.Plugins;

namespace _Test.WebAPI
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("get")]
        public IActionResult Get()
        {
            return Ok("ok");
        }

        [HttpPost("post")]
        public IActionResult Post(Body body)
        {
            ResultInfo result;
            try
            {
                result = ResultInfo<object>.Success("ok", JwtManager.DecodeToken(body.Jwt));
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
