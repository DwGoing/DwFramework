using System;
using System.Collections.Generic;

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
            return Ok();
        }

        [HttpPost("post")]
        public IActionResult Post(Body body)
        {
            ResultInfo result;
            try
            {
                return Ok(ResultInfo.Success<string>(JwtManager.GenerateToken("dwgoing", "ayou1209ayou1209ayou1209", new[] { "a", "b" }, customFields: new Dictionary<string, object>() { { "A", "a" } })));
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
