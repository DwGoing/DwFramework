using System;
using System.Threading;
using System.Threading.Tasks;

namespace DwFramework.Core.Helper
{
    public static class ThreadHelper
    {
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        public static Task CreateTask(Action taskAction)
        {
            var tcs = new TaskCompletionSource<object>();
            var thread = new Thread(() =>
            {
                try
                {
                    taskAction();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            thread.IsBackground = true;
            thread.Start();
            return tcs.Task;
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<T> taskAction)
        {
            var tcs = new TaskCompletionSource<T>();
            var thread = new Thread(() =>
            {
                try
                {
                    tcs.SetResult(taskAction());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            thread.IsBackground = true;
            thread.Start();
            return tcs.Task;
        }
    }
}
