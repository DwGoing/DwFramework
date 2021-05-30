using System;

namespace DwFramework.Core
{
    public partial class ResultInfo
    {
        public StatusCode Code { get; init; }
        public string Message { get; init; }
        public dynamic Data { get; init; }

        public ResultInfo() { }

        /// <summary>
        /// 创建结果信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultInfo Create(StatusCode code = StatusCode.OK, string message = "Success")
        {
            return new ResultInfo()
            {
                Code = code,
                Message = message != null ? message : code.GetDescription()
            };
        }

        /// <summary>
        /// 创建结果信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResultInfo Create<T>(StatusCode code = StatusCode.OK, string message = "Success", T data = default)
        {
            return new ResultInfo()
            {
                Code = code,
                Message = message != null ? message : code.GetDescription(),
                Data = data
            };
        }
    }
}
