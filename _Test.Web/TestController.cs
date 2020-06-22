using System;

using Microsoft.AspNetCore.Mvc;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace _Test.Web
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpPost("t")]
        public IActionResult Test(Body body)
        {

            var order = new
            {
                ID = Generater.GenerateUUID(),
                PayType = body.payType,
                OutTradeNo = body.outTradeNo,
                Fee = 0.01,
                TotalAmount = body.totalAmount
            };
            Console.WriteLine($"预下单信息:{order.ToJson()}");
            return Ok(ResultInfo.Success("ok"));
        }

        [HttpGet("x")]
        public IActionResult X()
        {
            return Ok("ok");
        }
    }


    public class Body
    {
        public int payType { get; set; }
        public string outTradeNo { get; set; }
        public string totalAmount { get; set; }
    }
}
