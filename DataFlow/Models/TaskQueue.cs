using System;
using System.Collections.Generic;

using DwFramework.Core.Helper;

namespace DwFramework.DataFlow
{
    public interface ITaskQueue
    {
        public void Enable(bool isEnable);
        public void AddTaskStartHandler(OnTaskStartHandler handler);
        public void RemoveTaskStartHandler(OnTaskStartHandler handler);
        public void ClearTaskStartHandlers();
        public void AddTaskEndHandler(OnTaskEndHandler handler);
        public void RemoveTaskEndHandler(OnTaskEndHandler handler);
        public void ClearTaskEndHandlers();
        public void AddInput(object input);
        public void ClearAllInputs();
        public object GetResult();
    }

    public delegate void OnTaskStartHandler(object input);
    public delegate void OnTaskEndHandler(object input, object output, object result);

    public class TaskQueue<TInput, TOutput, TResult> : ITaskQueue
    {
        private readonly ITaskHandler<TInput, TOutput> _taskHandler;
        private readonly IResultHandler<TOutput, TResult> _resultHandler;
        private readonly Queue<TInput> _inputs = new Queue<TInput>();
        private bool _isEnable = false;
        private TResult Result;
        private event OnTaskStartHandler OnTaskStart;
        private event OnTaskEndHandler OnTaskEnd;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="taskHandler"></param>
        /// <param name="resultHandler"></param>
        public TaskQueue(ITaskHandler<TInput, TOutput> taskHandler, IResultHandler<TOutput, TResult> resultHandler)
        {
            _taskHandler = taskHandler;
            _resultHandler = resultHandler;
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        /// <param name="isEnable"></param>
        public void Enable(bool isEnable)
        {
            _isEnable = isEnable;
            if (_isEnable) ThreadHelper.CreateTask(TaskHandler);
        }

        /// <summary>
        /// 添加任务开始时的处理
        /// </summary>
        /// <param name="handler"></param>
        public void AddTaskStartHandler(OnTaskStartHandler handler) => OnTaskStart += handler;

        /// <summary>
        /// 移除任务开始时的处理
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveTaskStartHandler(OnTaskStartHandler handler) => OnTaskStart -= handler;

        /// <summary>
        /// 移除所有任务开始时的处理
        /// </summary>
        public void ClearTaskStartHandlers() => OnTaskStart = null;

        /// <summary>
        /// 添加任务结束时的处理
        /// </summary>
        /// <param name="handler"></param>
        public void AddTaskEndHandler(OnTaskEndHandler handler) => OnTaskEnd += handler;

        /// <summary>
        /// 移除任务结束时的处理
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveTaskEndHandler(OnTaskEndHandler handler) => OnTaskEnd -= handler;

        /// <summary>
        /// 移除所有任务结束时的处理
        /// </summary>
        public void ClearTaskEndHandlers() => OnTaskEnd = null;

        /// <summary>
        /// 添加输入
        /// </summary>
        /// <param name="input"></param>
        public void AddInput(object input)
        {
            if (!(input is TInput)) throw new Exception("输入类型错误");
            _inputs.Enqueue((TInput)input);
            if (_isEnable) return;
            _isEnable = true;
            ThreadHelper.CreateTask(TaskHandler);
        }

        /// <summary>
        /// 移除所有输入
        /// </summary>
        public void ClearAllInputs()
        {
            _inputs.Clear();
        }
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public object GetResult()
        {
            return Result;
        }

        private void TaskHandler()
        {
            while (_isEnable && _inputs.Count > 0)
            {
                var input = _inputs.Dequeue();
                OnTaskStart.Invoke(input);
                var output = _taskHandler.Invoke(input);
                Result = _resultHandler.Invoke(output, Result);
                OnTaskEnd.Invoke(input, output, Result);
            }
            _isEnable = false;
        }
    }
}
