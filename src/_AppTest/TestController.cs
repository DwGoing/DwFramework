using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace _AppTest
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("t")]
        public IActionResult T()
        {
            return Ok("ok");
        }
    }
}