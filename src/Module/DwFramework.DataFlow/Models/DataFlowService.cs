using System;
using System.Collections.Generic;

using DwFramework.Core.Extensions;

namespace DwFramework.DataFlow
{
    public sealed class DataFlowService
    {
        private readonly Dictionary<string, TaskQueue> _taskQueues = new Dictionary<string, TaskQueue>();

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
        /// <param name="queueId"></param>
        /// <returns></returns>
        public TaskQueue<TInput, TOutput, TResult> CreateTaskQueue<TInput, TOutput, TResult>(Func<TInput, TOutput> taskHandler, Func<TOutput, TResult> resultHandler, out string queueId)
        {
            queueId = Guid.NewGuid().ToString();
            var queue = new TaskQueue<TInput, TOutput, TResult>(taskHandler, resultHandler);
            _taskQueues[queueId] = queue;
            return queue;
        }

        /// <summary>
        /// 获取任务队列
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queueId"></param>
        /// <returns></returns>
        public TaskQueue<TInput, TOutput, TResult> GetTaskQueue<TInput, TOutput, TResult>(string queueId)
        {
            if (!_taskQueues.ContainsKey(queueId)) return null;
            return (TaskQueue<TInput, TOutput, TResult>)_taskQueues[queueId];
        }

        /// <summary>
        /// 移除任务队列
        /// </summary>
        /// <param name="queueId"></param>
        public void RemoveTaskQueue(string queueId)
        {
            if (!_taskQueues.ContainsKey(queueId)) return;
            _taskQueues[queueId].ClearAllInputs();
            _taskQueues.Remove(queueId);
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
