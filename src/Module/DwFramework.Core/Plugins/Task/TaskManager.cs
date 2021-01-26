using System;
using System.Threading;
using System.Threading.Tasks;

namespace DwFramework.Core.Plugins
{
    public static class TaskManager
    {
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task CreateTask(Action action)
        {
            return Task.Run(action);
        }

        /// <summary>
        /// 创建限时任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeoutMilliSeconds"></param>
        /// <returns></returns>
        public static Task CreateTask(Action<CancellationToken> action, int timeoutMilliSeconds)
        {
            var cancellationToken = new CancellationTokenSource(timeoutMilliSeconds);
            var token = cancellationToken.Token;
            return Task.Run(() => action(token), token);
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task CreateTask(Action<CancellationToken> action, out CancellationTokenSource cancellationToken)
        {
            cancellationToken = new CancellationTokenSource();
            var token = cancellationToken.Token;
            return Task.Run(() => action(token), token);
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task CreateTask(Action<CancellationToken> action, CancellationTokenSource cancellationToken)
        {
            var token = cancellationToken.Token;
            return Task.Run(() => action(token), token);
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<T> action)
        {
            return Task.Run(action);
        }

        /// <summary>
        /// 创建限时任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="timeoutMilliSeconds"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<CancellationToken, T> action, int timeoutMilliSeconds)
        {
            var cancellationToken = new CancellationTokenSource(timeoutMilliSeconds);
            var token = cancellationToken.Token;
            return Task.Run(() => action(token), token);
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<CancellationToken, T> action, out CancellationTokenSource cancellationToken)
        {
            cancellationToken = new CancellationTokenSource();
            var token = cancellationToken.Token;
            return Task.Run(() => action(token), token);
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<CancellationToken, T> action, CancellationTokenSource cancellationToken)
        {
            var token = cancellationToken.Token;
            return Task.Run(() => action(token), token);
        }
    }
}
