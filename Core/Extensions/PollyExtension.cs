using System;
using System.Linq.Expressions;

using Polly;

namespace DwFramework.Core.Extensions
{
    public static class PollyExtension
    {
        public static T Retry<T>(this object func, Expression<Func<T, bool>> expression, int retryCount, Action<DelegateResult<T>, int> onRetry)
        {
            var t = func.GetType();
            var policy = Policy.HandleResult(expression.Compile()).Retry(retryCount, onRetry);
            return policy.Execute(() => { return default; });
        }
    }
}
