using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DwFramework.Core;

namespace DwFramework.WEB
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    public sealed class ExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled == false) context.Result = new ObjectResult(ResultInfo.Create(StatusCode.ERROR, context.Exception.Message));
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}