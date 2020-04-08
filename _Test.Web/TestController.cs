using Microsoft.AspNetCore.Mvc;

namespace _Test.Web
{
    [Route("[controller]")]
    public class TestController : Controller
    {
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("ok");
        }
    }
}
