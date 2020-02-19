using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Polly.Fallback;

namespace DwFramework.Core.Plugins
{
    public static class PollyPlugin
    {
        /// <summary>
        /// 异常后重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="retryCount"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static RetryPolicy RetryWhenException<TException>(int retryCount, Action<Exception, int> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            return builder.Retry(retryCount, onRetry);
        }

        /// <summary>
        /// 特定后结果重试
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="retryCount"></param>
        /// <param name="onRetry"></param>
        /// <returns></returns>
        public static RetryPolicy<TResult> RetryWhereResult<TResult>(Expression<Func<TResult, bool>> expression, int retryCount, Action<TResult, int> onRetry)
        {
            return Policy.HandleResult(expression.Compile()).Retry(retryCount, (res, index) =>
            {
                onRetry(res.Result, index);
            });
        }

        /// <summary>
        /// 异常后始终重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static RetryPolicy RetryForeverWhenException<TException>(Action<Exception, int> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            return builder.RetryForever(onRetry);
        }

        /// <summary>
        /// 特定结果后始终重试
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="onRetry"></param>
        /// <returns></returns>
        public static RetryPolicy<TResult> RetryForeverWhereResult<TResult>(Expression<Func<TResult, bool>> expression, Action<TResult, int> onRetry)
        {
            return Policy.HandleResult(expression.Compile()).RetryForever((res, index) =>
            {
                onRetry(res.Result, index);
            });
        }

        /// <summary>
        /// 异常后延时重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="retryCount"></param>
        /// <param name="spacingMilliseconds"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static RetryPolicy WaitAndRetryWhenException<TException>(int retryCount, long spacingMilliseconds, Action<Exception, TimeSpan> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            return builder.WaitAndRetry(retryCount, sleep => TimeSpan.FromMilliseconds(spacingMilliseconds), onRetry);
        }

        /// <summary>
        /// 异常后延时重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="spacingMilliseconds"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static RetryPolicy WaitAndRetryWhenException<TException>(long[] spacingMilliseconds, Action<Exception, TimeSpan> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            List<TimeSpan> sleeps = new List<TimeSpan>();
            foreach (var item in spacingMilliseconds)
            {
                sleeps.Add(TimeSpan.FromMilliseconds(item));
            }
            return builder.WaitAndRetry(sleeps, onRetry);
        }

        /// <summary>
        /// 特定结果后延时重试
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="spacingMilliseconds"></param>
        /// <param name="onRetry"></param>
        /// <returns></returns>
        public static RetryPolicy<TResult> WaitAndRetryWhereResult<TResult>(Expression<Func<TResult, bool>> expression, long[] spacingMilliseconds, Action<TResult, TimeSpan> onRetry)
        {
            List<TimeSpan> sleeps = new List<TimeSpan>();
            foreach (var item in spacingMilliseconds)
            {
                sleeps.Add(TimeSpan.FromMilliseconds(item));
            }
            return Policy.HandleResult(expression.Compile()).WaitAndRetry(sleeps, (res, ts) =>
            {
                onRetry(res.Result, ts);
            });
        }

        /// <summary>
        /// 异常后熔断
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="allowExceptions"></param>
        /// <param name="millisecondsOfBreak"></param>
        /// <param name="onBreak"></param>
        /// <param name="onReset"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static CircuitBreakerPolicy CircuitBreakerWhenException<TException>(int allowCount, long millisecondsOfBreak, Action<Exception, TimeSpan> onBreak, Action onReset, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            return builder.CircuitBreaker(allowCount, TimeSpan.FromMilliseconds(millisecondsOfBreak), onBreak, onReset);
        }

        /// <summary>
        /// 特定结果后熔断
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="allowExceptions"></param>
        /// <param name="millisecondsOfBreak"></param>
        /// <param name="onBreak"></param>
        /// <param name="onReset"></param>
        /// <returns></returns>
        public static CircuitBreakerPolicy<TResult> CircuitBreakerWhenResult<TResult>(Expression<Func<TResult, bool>> expression, int allowExceptions, long millisecondsOfBreak, Action<TResult, TimeSpan> onBreak, Action onReset)
        {
            return Policy.HandleResult(expression.Compile()).CircuitBreaker(allowExceptions, TimeSpan.FromMilliseconds(millisecondsOfBreak), (res, ts) =>
            {
                onBreak(res.Result, ts);
            }, onReset);
        }

        /// <summary>
        /// 异常后使用备用
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="fallbackAction"></param>
        /// <param name="onFallback"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static FallbackPolicy FallbackWhenException<TException>(Action fallbackAction, Action<Exception> onFallback, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            return builder.Fallback(fallbackAction, onFallback);
        }

        /// <summary>
        /// 特定结果后使用备用
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="fallbackAction"></param>
        /// <param name="onFallback"></param>
        /// <returns></returns>
        public static FallbackPolicy<TResult> FallbackWhenException<TResult>(Expression<Func<TResult, bool>> expression, Func<TResult> fallbackAction, Action<TResult> onFallback)
        {
            return Policy.HandleResult(expression.Compile()).Fallback(fallbackAction, (res) =>
            {
                onFallback(res.Result);
            });
        }
    }
}
