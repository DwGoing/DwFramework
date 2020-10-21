using System;

namespace DwFramework.Core.Plugins
{
    public static class ParallelManager
    {
        /// <summary>
        /// 创建并行任务
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public static ParallelTask Create(params Action[] actions) => new ParallelTask(actions);
    }
}
