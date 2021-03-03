namespace DwFramework.Core
{
    public partial class ResultInfo
    {
        public const int OK = 200;
        public const int ERROR = 400;

        public int Code { get; init; }
        public string Message { get; init; }
        public dynamic Data { get; init; }

        public ResultInfo() { }

        /// <summary>
        /// 创建结果信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultInfo Create(int code = OK, string message = "Success")
        {
            return new ResultInfo()
            {
                Code = code,
                Message = message
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
        public static ResultInfo Create<T>(int code = OK, string message = "Success", T data = default)
        {
            return new ResultInfo()
            {
                Code = code,
                Message = message,
                Data = data
            };
        }
    }
}
