using System;
using System.Collections.Generic;

namespace DwFramework.DataFlow
{
    public class DataFlowService
    {
        private readonly Dictionary<string, ITaskQueue> _taskQueues = new Dictionary<string, ITaskQueue>();

        public void CreateTaskQueue<TInput, TOutput, TResult>(string key, ITaskHandler<TInput, TOutput> taskHandler, IResultHandler<TOutput, TResult> resultHandler)
        {
            _taskQueues[key] = new TaskQueue<TInput, TOutput, TResult>(taskHandler, resultHandler);
        }

        public void AddInput<TInput>(string key, TInput input)
        {
            if (!_taskQueues.ContainsKey(key)) throw new Exception($"Key:{key}不存在");
            _taskQueues[key].AddInput(input);
        }

        public object GetResult(string key)
        {
            if (!_taskQueues.ContainsKey(key)) throw new Exception($"Key:{key}不存在");
            return _taskQueues[key].GetResult();
        }
    }
}
