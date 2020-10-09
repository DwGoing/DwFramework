using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DwFramework.WebAPI.Jwt;

namespace _AppTest
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        public TestController()
        {
        }

        [HttpGet("a")]
        public IActionResult A()
        {
            return Ok(JwtManager.GenerateToken("dwgoing", "0123456789abcdef", customFields: new Dictionary<string, object>() {
                { "A","a" },
                {"B",1 },
                {"C",new {Name="DDD"} }
            }));
        }
    }
}
