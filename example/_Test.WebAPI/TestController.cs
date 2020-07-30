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
                result = ResultInfo.Success(JwtManager.DecodeToken(body.Jwt), "ok");
            }
            catch (Exception ex)
            {
                result = ResultInfo.Fail(message: ex.Message);
            }
            return Ok(result);
        }
    }

    public class Body
    {
        public string Jwt { get; set; }
    }
}
