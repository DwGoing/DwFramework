using System;

namespace DwFramework.Core
{
    public class ExceptionBase : Exception
    {
        public StatusCode Code { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ExceptionBase(StatusCode code, string message = null, Exception innerException = null) : base(message ?? code.GetDescription(), innerException)
        {
            Code = code;
        }
    }
}