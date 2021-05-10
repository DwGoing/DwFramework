using System;

namespace DwFramework.Core.Entities
{
    public abstract class ExceptionBase : Exception
    {
        public StatusCode Code { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ExceptionBase(StatusCode code, string message, Exception innerException = null) : base(message, innerException)
        {
            Code = code;
        }
    }
}