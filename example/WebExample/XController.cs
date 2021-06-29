using System;
using Microsoft.AspNetCore.Mvc;
using DwFramework.Web.JWT;

namespace WebExample
{
    [ApiController]
    [Route("x")]
    public class XController : Controller
    {
        public XController()
        {
        }

        [HttpGet("t1")]
        public IActionResult T1()
        {
            return Ok(JwtManager.Generate("dwgoing", "dsfjoihnoisdhf823b4iu834h"));
        }

        [HttpGet("t2")]
        public IActionResult T2(string token)
        {
            return Ok(JwtManager.Decode(token));
        }
    }
}