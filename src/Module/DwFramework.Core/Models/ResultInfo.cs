namespace DwFramework.Core
{
    public partial class ResultInfo
    {
        public const int Ok = 200;
        public const int Error = 400;

        public int Code { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultInfo Success(string message)
        {
            return new ResultInfo()
            {
                Code = Ok,
                Message = message
            };
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ResultInfo Fail(string message, int code = Error)
        {
            return new ResultInfo()
            {
                Code = code,
                Message = message
            };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResultInfo Success<T>(string message, T data)
        {
            var result = Success(message);
            result.Data = data;
            return result;
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ResultInfo Fail<T>(string message, T data, int code = Error)
        {
            var result = Fail(message, code);
            result.Data = data;
            return result;
        }
    }
}
