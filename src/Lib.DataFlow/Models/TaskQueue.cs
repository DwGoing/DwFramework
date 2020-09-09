using System;
using System.Collections.Concurrent;

using DwFramework.Core.Plugins;

namespace DwFramework.DataFlow
{
    public class TaskQueue<TInput, TOutput, TResult> : ITaskQueue
    {
        public string ID => Generater.GenerateGUID().ToString();

        private readonly Func<TInput, TOutput> _taskHandler;
        private readonly Func<TOutput, TResult> _resultHandler;
        private readonly ConcurrentQueue<(string id, TInput input)> _inputs;
        private bool _isExcuting = false;
        private (string id, TInput input) _currentInput = (null, default);
        private TResult _result;

        public event Action<TResult, string, TInput> OnExcuting;
        public event Action<string, TInput, TOutput, TResult> OnExcuted;
        public event Action<string, TInput, Exception> OnExcuteError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="taskHandler"></param>
        /// <param name="resultHandler"></param>
        public TaskQueue(Func<TInput, TOutput> taskHandler, Func<TOutput, TResult> resultHandler)
        {
            _taskHandler = taskHandler;
            _resultHandler = resultHandler;
            _inputs = new ConcurrentQueue<(string id, TInput input)>();
        }

        /// <summary>
        /// 添加输入
        /// </summary>
        /// <param name="input"></param>
        public string AddInput(object input)
        {
            if (!(input is TInput)) throw new Exception($"输入类型不是{nameof(TInput)}");
            string id = Generater.GenerateGUID().ToString();
            _inputs.Enqueue((id, (TInput)input));
            if (!_isExcuting)
            {
                _isExcuting = true;
                TaskManager.CreateTask(Excute);
            }
            return id;
        }

        /// <summary>
        /// 移除所有输入
        /// </summary>
        public void ClearAllInputs() => _inputs.Clear();

        /// <summary>
        /// 执行任务
        /// </summary>
        public void Excute()
        {
            while (!_inputs.IsEmpty)
            {
                if (!_inputs.TryDequeue(out var item)) continue;
                try
                {
                    OnExcuting?.Invoke(_result, item.id, item.input);
                    var output = _taskHandler.Invoke(item.input);
                    _result = _resultHandler.Invoke(output);
                    OnExcuted?.Invoke(item.id, item.input, output, _result);
                }
                catch (Exception ex)
                {
                    OnExcuteError?.Invoke(item.id, item.input, ex);
                }
            }
            _isExcuting = false;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public object GetResult(out string inputId)
        {
            inputId = _currentInput.id;
            return _result;
        }
    }
}
