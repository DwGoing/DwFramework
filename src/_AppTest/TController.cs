using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace _AppTest
{
    [ApiController]
    [Route("t")]
    public class TController : Controller
    {
        public TController()
        {
        }

        [HttpGet("tt")]
        public IActionResult TT()
        {
            return Ok("ok");
        }
    }
}
