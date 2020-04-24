using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DwFramework.DataFlow
{
    public interface ITaskQueue
    {
        public void Enable(bool isEnable);
        public void AddInput(object input);
        public object GetResult();
    }

    public class TaskQueue<TInput, TOutput, TResult> : ITaskQueue
    {
        private readonly ITaskHandler<TInput, TOutput> _taskHandler;
        private readonly IResultHandler<TOutput, TResult> _resultHandler;
        private readonly Queue<TInput> _inputs = new Queue<TInput>();
        private bool _isEnable = false;
        private TResult Result;

        public TaskQueue(ITaskHandler<TInput, TOutput> taskHandler, IResultHandler<TOutput, TResult> resultHandler)
        {
            _taskHandler = taskHandler;
            _resultHandler = resultHandler;
        }

        public void Enable(bool isEnable)
        {
            _isEnable = isEnable;
            if (_isEnable) Task.Run(TaskHandler);
        }

        public void AddInput(object input)
        {
            if (!(input is TInput)) throw new Exception("输入类型错误");
            _inputs.Enqueue((TInput)input);
            if (_isEnable) return;
            _isEnable = true;
            Task.Run(TaskHandler);
        }

        public object GetResult()
        {
            return Result;
        }

        private void TaskHandler()
        {
            while (_isEnable && _inputs.Count > 0)
            {
                var input = _inputs.Dequeue();
                var output = _taskHandler.Invoke(input);
                Result = _resultHandler.Invoke(output, Result);
            }
            _isEnable = false;
        }
    }
}
