namespace DwFramework.Core
{
    public class ResultCode
    {
        public const int Ok = 200;
        public const int Error = 400;
    }

    public class ResultInfo
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }

        public static ResultInfo Success(string message = null)
        {
            return new ResultInfo()
            {
                Code = ResultCode.Ok,
                Message = message ?? "调用成功"
            };
        }

        public static ResultInfo Fail(string message = null, int code = ResultCode.Error)
        {
            return new ResultInfo()
            {
                Code = code,
                Message = message ?? "调用失败"
            };
        }

        public static ResultInfo Success<T>(T data, string message = null)
        {
            var result = Success(message);
            result.Data = data;
            return result;
        }

        public static ResultInfo Fail<T>(T data, string message = null, int code = ResultCode.Error)
        {
            var result = Fail(message, code);
            result.Data = data;
            return result;
        }
    }
}
