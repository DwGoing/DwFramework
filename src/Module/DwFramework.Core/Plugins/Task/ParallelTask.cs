using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DwFramework.Core.Extensions;

namespace DwFramework.Core.Plugins
{
    public sealed class ParallelTask
    {
        private readonly List<Action> _actions;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParallelTask()
        {
            _actions = new List<Action>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="actions"></param>
        public ParallelTask(params Action[] actions)
        {
            _actions = new List<Action>(actions);
        }

        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="action"></param>
        public void AddAction(Action action) => _actions.Add(action);

        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="actions"></param>
        public void AddActionRange(IEnumerable<Action> actions) => _actions.AddRange(actions);

        /// <summary>
        /// 开始任务
        /// </summary>
        public void Start(Action<Exception> onException = null)
        {
            try
            {
                Parallel.Invoke(_actions.ToArray());
            }
            catch (AggregateException ex)
            {
                ex.InnerExceptions.ForEach(item => onException?.Invoke(item));
            }
        }
    }
}
