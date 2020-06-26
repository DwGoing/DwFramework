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
        /// <param name="taskAction"></param>
        /// <returns></returns>
        public static Task CreateTask(Action taskAction)
        {
            return Task.Factory.StartNew(taskAction);
        }

        /// <summary>
        /// 创建限时任务
        /// </summary>
        /// <param name="taskAction"></param>
        /// <param name="timeoutMilliSeconds"></param>
        /// <returns></returns>
        public static Task CreateTask(Action<CancellationToken> taskAction, int timeoutMilliSeconds)
        {
            var cancellationToken = new CancellationTokenSource(timeoutMilliSeconds);
            return Task.Factory.StartNew(token =>
            {
                taskAction((CancellationToken)token);
            }, cancellationToken.Token);
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <param name="taskAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task CreateTask(Action<CancellationToken> taskAction, out CancellationTokenSource cancellationToken)
        {
            cancellationToken = new CancellationTokenSource();
            return Task.Factory.StartNew(token =>
            {
                taskAction((CancellationToken)token);
            }, cancellationToken.Token);
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <param name="taskAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task CreateTask(Action<CancellationToken> taskAction, CancellationTokenSource cancellationToken)
        {
            return Task.Factory.StartNew(token =>
            {
                taskAction((CancellationToken)token);
            }, cancellationToken.Token);
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<T> taskAction)
        {
            return Task.Factory.StartNew(taskAction);
        }

        /// <summary>
        /// 创建限时任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskAction"></param>
        /// <param name="timeoutMilliSeconds"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<CancellationToken, T> taskAction, int timeoutMilliSeconds)
        {
            var cancellationToken = new CancellationTokenSource(timeoutMilliSeconds);
            return Task.Factory.StartNew(token =>
            {
                return taskAction((CancellationToken)token);
            }, cancellationToken.Token);
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<CancellationToken, T> taskAction, out CancellationTokenSource cancellationToken)
        {
            cancellationToken = new CancellationTokenSource();
            return Task.Factory.StartNew(token =>
            {
                return taskAction((CancellationToken)token);
            }, cancellationToken.Token);
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<CancellationToken, T> taskAction, CancellationTokenSource cancellationToken)
        {
            return Task.Factory.StartNew(token =>
            {
                return taskAction((CancellationToken)token);
            }, cancellationToken.Token);
        }
    }
}
