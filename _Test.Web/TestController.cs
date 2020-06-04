using System;

using Microsoft.AspNetCore.Mvc;

using DwFramework.Core;

namespace _Test.Web
{
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("t")]
        public IActionResult Test()
        {
            return Ok(ResultInfo.Success("ok"));
        }
    }
}
