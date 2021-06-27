using System;
using System.ComponentModel;

namespace DwFramework.Core
{
    /// <summary>
    /// 状态码
    /// </summary>
    public enum StatusCode
    {
        [Description("Ok")]
        Ok = 200,
        [Description("未知错误")]
        Error = 40000
    }
}
