using System;
using System.Threading;

using Microsoft.AspNetCore.Mvc;

using DwFramework.Core;
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
            Console.WriteLine(RequestId.Get());
            Thread.Sleep(5000);
            Console.WriteLine(RequestId.Get());
            return Ok();
        }

        [HttpGet("g")]
        public IActionResult G()
        {
            return Ok(RequestId.Get());
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
