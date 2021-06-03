using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DwFramework.Core;

namespace DwFramework.WEB
{
    /// <summary>
    /// 结果过滤器
    /// </summary>
    public sealed class ResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is EmptyResult) context.Result = new ObjectResult(ResultInfo.Create());
            else if (context.Result is ObjectResult)
            {
                var result = context.Result as ObjectResult;
                if (result.Value is ResultInfo) context.Result = new ObjectResult(result.Value);
                else context.Result = new ObjectResult(ResultInfo.Create<object>(data: result?.Value));
            }
            _ = await next();
        }
    }
}