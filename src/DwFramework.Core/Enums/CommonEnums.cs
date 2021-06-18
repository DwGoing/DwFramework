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

    /// <summary>
    /// 压缩类型
    /// </summary>
    public enum CompressType
    {
        Unknow = 0,
        Brotli = 1,
        GZip = 2,
        LZ4 = 3
    }
}
