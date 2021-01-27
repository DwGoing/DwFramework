using Microsoft.AspNetCore.Mvc;

namespace DwFramework.Example.WebAPI
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
            return Ok();
        }
    }
}
