using System;
using System.Collections.Generic;

namespace DwFramework.Core.Plugins
{
    public class Transaction
    {
        public string ID;

        private List<(Action Action, Action<Exception> RollbackAction)> _actions = new List<(Action Action, Action<Exception> RollbackAction)>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public Transaction()
        {
            ID = Generater.GenerateGUID().ToString();
        }

        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="action"></param>
        /// <param name="rollbackAction"></param>
        public void AddAction(Action action, Action<Exception> rollbackAction)
        {
            _actions.Add((action, rollbackAction));
        }

        /// <summary>
        /// 清除所有操作
        /// </summary>
        public void ClearAction()
        {
            _actions.Clear();
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        public void Invoke()
        {
            foreach (var item in _actions)
            {
                try
                {
                    item.Action.Invoke();
                }
                catch (Exception ex)
                {
                    item.RollbackAction.Invoke(ex);
                    break;
                }
            }
        }
    }
}
