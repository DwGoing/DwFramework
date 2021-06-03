using System;
using Microsoft.AspNetCore.Mvc;

namespace WEBExample
{
    [ApiController]
    [Route("x")]
    public class XController : Controller
    {
        public XController()
        {
        }

        [HttpGet("ok")]
        public IActionResult Get()
        {
            return Ok("ok");
        }
    }
}