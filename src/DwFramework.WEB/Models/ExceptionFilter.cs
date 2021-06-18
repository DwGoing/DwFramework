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
            if (context.ExceptionHandled == false)
                context.Result = new ObjectResult(context.Exception is ExceptionBase ?
                    ResultInfo.Create(((ExceptionBase)context.Exception).Code, ((ExceptionBase)context.Exception).Message)
                    : ResultInfo.Create(StatusCode.Error, context.Exception.InnerException != null ? context.Exception.InnerException.Message : context.Exception.Message));
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}