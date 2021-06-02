using System;
using Microsoft.AspNetCore.Mvc;

namespace CoreExample
{
    [ApiController]
    [Route("x")]
    public class XController : Controller
    {
        public XController(I a)
        {
            var x = a.Do(10, 20);
        }

        [HttpGet("ok")]
        public IActionResult Get()
        {
            return Ok("ok");
        }
    }
}