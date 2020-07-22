using System;
using System.Collections.Generic;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.DataFlow
{
    public class DataFlowService : BaseService
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
        public string CreateTaskQueue<TInput, TOutput, TResult>(ITaskHandler<TInput, TOutput> taskHandler, IResultHandler<TOutput, TResult> resultHandler)
        {
            var key = Generater.GenerateGUID().ToString();
            _taskQueues[key] = new TaskQueue<TInput, TOutput, TResult>(taskHandler, resultHandler);
            return key;
        }

        /// <summary>
        /// 移除任务队列
        /// </summary>
        /// <param name="key"></param>
        public void RemoveTaskQueue(string key)
        {
            RequireKey(key);
            _taskQueues.Remove(key);
        }

        /// <summary>
        /// 移除所有任务队列
        /// </summary>
        public void ClearTaskQueues()
        {
            _taskQueues.Clear();
        }

        /// <summary>
        /// 添加任务开始时的处理
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void AddTaskStartHandler<TInput>(string key, OnTaskStartHandler handler)
        {
            RequireKey(key);
            _taskQueues[key].AddTaskStartHandler(handler);
        }

        /// <summary>
        /// 移除任务开始时的处理
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void RemoveTaskStartHandler<TInput>(string key, OnTaskStartHandler handler)
        {
            RequireKey(key);
            _taskQueues[key].RemoveTaskStartHandler(handler);
        }

        /// <summary>
        /// 移除所有任务开始时的处理
        /// </summary>
        /// <param name="key"></param>
        public void ClearTaskStartHandlers(string key)
        {
            RequireKey(key);
            _taskQueues[key].ClearTaskStartHandlers();
        }

        /// <summary>
        /// 添加任务结束时的处理
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void AddTaskEndHandler<TInput, TOutput, TResult>(string key, OnTaskEndHandler handler)
        {
            RequireKey(key);
            _taskQueues[key].AddTaskEndHandler(handler);
        }

        /// <summary>
        /// 移除任务结束时的处理
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void RemoveTaskEndHandler<TInput, TOutput, TResult>(string key, OnTaskEndHandler handler)
        {
            RequireKey(key);
            _taskQueues[key].RemoveTaskEndHandler(handler);
        }

        /// <summary>
        /// 移除所有任务结束时的处理
        /// </summary>
        /// <param name="key"></param>
        public void ClearTaskEndHandlers(string key)
        {
            RequireKey(key);
            _taskQueues[key].ClearTaskEndHandlers();
        }

        /// <summary>
        /// 添加输入
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="key"></param>
        /// <param name="input"></param>
        public void AddInput<TInput>(string key, TInput input)
        {
            RequireKey(key);
            _taskQueues[key].AddInput(input);
        }

        /// <summary>
        /// 移除所有输入
        /// </summary>
        /// <param name="key"></param>
        public void ClearInput(string key)
        {
            RequireKey(key);
            _taskQueues[key].ClearAllInputs();
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetResult(string key)
        {
            RequireKey(key);
            return _taskQueues[key].GetResult();
        }

        /// <summary>
        /// 验证Key
        /// </summary>
        /// <param name="key"></param>
        private void RequireKey(string key)
        {
            if (!_taskQueues.ContainsKey(key)) throw new Exception($"Key:{key}不存在");
        }
    }
}
