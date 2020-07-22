using System;

using Microsoft.AspNetCore.Mvc;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.Web;
using DwFramework.Web.Plugins;

namespace _Test.Web
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="body">数据</param>
        /// <returns></returns>
        [HttpPost("t")]
        public IActionResult Test(Body body)
        {
            ResultInfo result;
            try
            {
                result = ResultInfo<object>.Success("ok", JwtManager.DecodeToken(body.Jwt));
            }
            catch (Exception ex)
            {
                result = ResultInfo.Fail(ex.Message);
            }
            return Ok(result);
        }
    }

    public class Body
    {
        public string Jwt { get; set; }
    }
}
