using System;
using System.Linq.Expressions;

using Polly;

namespace DwFramework.Core
{
    public class ResultCode
    {
        public const int Ok = 200;
        public const int Error = 400;
    }

    public class ResultInfo
    {
        public int Code;
        public string Message;

        public static ResultInfo Success(string message)
        {
            return new ResultInfo()
            {
                Code = ResultCode.Ok,
                Message = message
            };
        }

        public static ResultInfo Fail(string message, int code = ResultCode.Error)
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
                Code = ResultCode.Ok,
                Message = message,
                Data = data
            };
        }

        public static ResultInfo<T> Fail(string message, T data, int code = ResultCode.Error)
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
