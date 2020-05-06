using System;

using Microsoft.AspNetCore.Mvc;

using DwFramework.Core;

namespace _Test.Web
{
    [Route("[controller]")]
    public class TestController : Controller
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(ResultInfo.Success("ok"));
        }
    }
}
