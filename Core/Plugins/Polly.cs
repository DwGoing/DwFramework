using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Polly;

namespace DwFramework.Core.Plugins
{
    public static class PollyPlugin
    {
        /// <summary>
        /// 异常后重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        /// <param name="retryCount"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        public static void RetryWhenException<TException>(Action action, int retryCount, Action<Exception, int> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            var policy = builder.Retry(retryCount, onRetry);
            policy.Execute(action);
        }

        /// <summary>
        /// 异常后重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="retryCount"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TResult RetryWhenException<TException, TResult>(Func<TResult> func, int retryCount, Action<Exception, int> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            var policy = builder.Retry(retryCount, onRetry);
            return policy.Execute(func);
        }

        /// <summary>
        /// 特定后结果重试
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="expression"></param>
        /// <param name="retryCount"></param>
        /// <param name="onRetry"></param>
        /// <returns></returns>
        public static TResult RetryWhereResult<TResult>(Func<TResult> func, Expression<Func<TResult, bool>> expression, int retryCount, Action<TResult, int> onRetry)
        {
            var policy = Policy.HandleResult(expression.Compile()).Retry(retryCount, (res, i) =>
            {
                onRetry(res.Result, i);
            });
            return policy.Execute(func);
        }

        /// <summary>
        /// 异常后始终重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        public static void RetryForeverWhenException<TException>(Action action, Action<Exception, int> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            var policy = builder.RetryForever(onRetry);
            policy.Execute(action);
        }

        /// <summary>
        /// 异常后始终重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TResult RetryForeverWhenException<TException, TResult>(Func<TResult> func, Action<Exception, int> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            var policy = builder.RetryForever(onRetry);
            return policy.Execute(func);
        }

        /// <summary>
        /// 特定结果后始终重试
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="expression"></param>
        /// <param name="onRetry"></param>
        /// <returns></returns>
        public static TResult RetryForeverWhereResult<TResult>(Func<TResult> func, Expression<Func<TResult, bool>> expression, Action<TResult, int> onRetry)
        {
            var policy = Policy.HandleResult(expression.Compile()).RetryForever((res, i) =>
            {
                onRetry(res.Result, i);
            });
            return policy.Execute(func);
        }

        /// <summary>
        /// 异常后延时重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        /// <param name="retryCount"></param>
        /// <param name="spacingMilliseconds"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        public static void WaitAndRetryWhenException<TException>(Action action, int retryCount, long spacingMilliseconds, Action<Exception, TimeSpan> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
        {
            PolicyBuilder builder = null;
            if (expression == null)
                builder = Policy.Handle<TException>();
            else
                builder = Policy.Handle(expression.Compile());
            var policy = builder.WaitAndRetry(retryCount, sleep => TimeSpan.FromMilliseconds(spacingMilliseconds), onRetry);
            policy.Execute(action);
        }

        /// <summary>
        /// 异常后延时重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        /// <param name="spacingMilliseconds"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        public static void WaitAndRetryWhenException<TException>(Action action, long[] spacingMilliseconds, Action<Exception, TimeSpan> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
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
            var policy = builder.WaitAndRetry(sleeps, onRetry);
            policy.Execute(action);
        }

        /// <summary>
        /// 异常后延时重试
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="retryCount"></param>
        /// <param name="spacingMilliseconds"></param>
        /// <param name="onRetry"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TResult WaitAndRetryWhenException<TException, TResult>(Func<TResult> func, long[] spacingMilliseconds, Action<Exception, TimeSpan> onRetry, Expression<Func<TException, bool>> expression = null) where TException : Exception
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
            var policy = builder.WaitAndRetry(sleeps, onRetry);
            return policy.Execute(func);
        }

        /// <summary>
        /// 特定结果后延时重试
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="expression"></param>
        /// <param name="retryCount"></param>
        /// <param name="spacingMilliseconds"></param>
        /// <param name="onRetry"></param>
        /// <returns></returns>
        public static TResult WaitAndRetryWhereResult<TResult>(Func<TResult> func, Expression<Func<TResult, bool>> expression, long[] spacingMilliseconds, Action<TResult, TimeSpan> onRetry)
        {
            List<TimeSpan> sleeps = new List<TimeSpan>();
            foreach (var item in spacingMilliseconds)
            {
                sleeps.Add(TimeSpan.FromMilliseconds(item));
            }
            var policy = Policy.HandleResult(expression.Compile()).WaitAndRetry(sleeps, (res, time) =>
            {
                onRetry(res.Result, time);
            });
            return policy.Execute(func);
        }
    }
}
