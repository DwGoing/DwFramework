using System;
using System.Collections.Generic;

using DwFramework.Extensions.Core;

namespace DwFramework.DataFlow
{
    public sealed class DataFlowService
    {
        private readonly Dictionary<string, ITaskQueue> _taskQueues = new Dictionary<string, ITaskQueue>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataFlowService()
        {

        }

        /// <summary>
        /// 创建任务队列
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="taskHandler"></param>
        /// <param name="resultHandler"></param>
        /// <returns></returns>
        public string CreateTaskQueue<TInput, TOutput, TResult>(Func<TInput, TOutput> taskHandler, Func<TOutput, TResult> resultHandler)
        {
            var taskQueue = new TaskQueue<TInput, TOutput, TResult>(taskHandler, resultHandler);
            _taskQueues[taskQueue.ID] = taskQueue;
            return taskQueue.ID;
        }

        /// <summary>
        /// 获取任务队列
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ITaskQueue GetTaskQueue(string key)
        {
            if (!_taskQueues.ContainsKey(key)) return null;
            return _taskQueues[key];
        }

        /// <summary>
        /// 移除任务队列
        /// </summary>
        /// <param name="key"></param>
        public void RemoveTaskQueue(string key)
        {
            if (!_taskQueues.ContainsKey(key)) return;
            _taskQueues[key].ClearAllInputs();
            _taskQueues.Remove(key);
        }

        /// <summary>
        /// 移除所有任务队列
        /// </summary>
        public void ClearTaskQueues() => _taskQueues.ForEach(item => RemoveTaskQueue(item.Key));

        /// <summary>
        /// 添加输入
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="key"></param>
        /// <param name="input"></param>
        public string AddInput<TInput>(string key, TInput input)
        {
            if (!_taskQueues.ContainsKey(key)) throw new Exception("该队列不存在");
            return _taskQueues[key].AddInput(input);
        }

        /// <summary>
        /// 移除所有输入
        /// </summary>
        /// <param name="key"></param>
        public void ClearInput(string key)
        {
            if (!_taskQueues.ContainsKey(key)) throw new Exception("该队列不存在");
            _taskQueues[key].ClearAllInputs();
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetResult(string key, out string inputId)
        {
            if (!_taskQueues.ContainsKey(key)) throw new Exception("该队列不存在");
            return _taskQueues[key].GetResult(out inputId);
        }
    }
}
