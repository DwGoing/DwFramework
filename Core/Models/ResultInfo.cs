namespace DwFramework.Core.Models
{
    public class ResultInfo
    {
        public int Code;
        public string Message;

        public static ResultInfo Success(string message)
        {
            return new ResultInfo()
            {
                Code = 200,
                Message = message
            };
        }

        public static ResultInfo Fail(string message, int code = 400)
        {
            return new ResultInfo()
            {
                Code = code,
                Message = message
            };
        }
    }

    public class ResultInfo<T> : ResultInfo
    {
        public T Data;

        public static ResultInfo<T> Success(string message, T data)
        {
            return new ResultInfo<T>()
            {
                Code = 200,
                Message = message,
                Data = data
            };
        }

        public static ResultInfo<T> Fail(string message, T data, int code = 400)
        {
            return new ResultInfo<T>()
            {
                Code = code,
                Message = message,
                Data = data
            };
        }
    }
}
