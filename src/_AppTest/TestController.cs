using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using DwFramework.WebAPI.Plugins;

namespace _AppTest
{
    [ApiVersionNeutral]
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpPost("file")]
        public async Task<IActionResult> File(IFormFile file)
        {
            if (file == null) throw new Exception("数据为空");
            if (!Directory.Exists("Upload")) Directory.CreateDirectory("Upload");
            using var stream = file.OpenReadStream();
            await WriteFileAsync(stream, Path.Combine("./Upload", file.FileName));
            return Ok("ok");
        }

        private static async Task<int> WriteFileAsync(Stream stream, string path)
        {
            const int FILE_WRITE_SIZE = 84975;//写出缓冲区大小
            int writeCount = 0;
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, FILE_WRITE_SIZE, true))
            {
                byte[] byteArr = new byte[FILE_WRITE_SIZE];
                int readCount = 0;
                while ((readCount = await stream.ReadAsync(byteArr, 0, byteArr.Length)) > 0)
                {
                    await fileStream.WriteAsync(byteArr, 0, readCount);
                    writeCount += readCount;
                }
            }
            return writeCount;
        }
    }
}

namespace _AppTest.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("b")]
        public IActionResult B()
        {
            return Ok(HttpContext.GetRequestedApiVersion().ToString());
        }
    }
}

namespace _AppTest.v2
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("c")]
        public IActionResult C()
        {
            return Ok(HttpContext.GetRequestedApiVersion().ToString());
        }
    }
}
