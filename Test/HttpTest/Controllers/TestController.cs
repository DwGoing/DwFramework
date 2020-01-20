using Microsoft.AspNetCore.Mvc;

namespace Test
{
    public class TestRequest
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("test1")]
        public ActionResult TestMethod1(string str)
        {
            return Ok(str);
        }

        [HttpPost("test2")]
        public ActionResult Post([FromBody] TestRequest request)
        {
            return Ok(request);
        }
    }
}
