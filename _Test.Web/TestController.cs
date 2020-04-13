using System;

using Microsoft.AspNetCore.Mvc;

namespace _Test.Web
{
    [Route("[controller]")]
    public class TestController : Controller
    {
        [HttpGet("test")]
        public ActionResult Test()
        {
            throw new Exception("异常测试");
            return Ok("ok");
        }
    }
}
