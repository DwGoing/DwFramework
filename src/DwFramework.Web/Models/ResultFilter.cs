using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DwFramework.Core;

namespace DwFramework.Web
{
    /// <summary>
    /// 结果过滤器
    /// </summary>
    public sealed class ResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is EmptyResult) context.Result = new ObjectResult(ResultInfo.Create());
            else if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value is ResultInfo resultInfo) context.Result = new ObjectResult(resultInfo);
                else
                {
                    objectResult.StatusCode ??= 200;
                    context.Result = new ObjectResult(ResultInfo.Create(
                        (StatusCode)objectResult.StatusCode,
                        objectResult.StatusCode == 200 ? "Success" : context.Result.GetType().Name.Replace("ObjectResult", ""),
                        objectResult.Value)
                    );
                }
            }
            _ = await next();
        }
    }
}