using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using DwFramework.WebAPI.Plugins;

namespace _AppTest
{
    [ApiVersionNeutral]
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("a")]
        public IActionResult A()
        {
            return Ok(HttpContext.GetRequestedApiVersion().ToString());
        }
    }
}

namespace _AppTest.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("b")]
        public IActionResult B()
        {
            return Ok(HttpContext.GetRequestedApiVersion().ToString());
        }
    }
}

namespace _AppTest.v2
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("c")]
        public IActionResult C()
        {
            return Ok(HttpContext.GetRequestedApiVersion().ToString());
        }
    }
}
