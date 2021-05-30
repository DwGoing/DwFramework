using System;

using DwFramework.Core.Entities;

namespace DwFramework.Core.Plugins
{
    public static class MemoryCacheExtension
    {
        // /// <summary>
        // /// 注册MemoryCache服务
        // /// </summary>
        // /// <param name="host"></param>
        // /// <param name="storeCount"></param>
        // /// <param name="isGlobal"></param>
        // /// <returns></returns>
        // public static ServiceHost RegisterMemoryCache(this ServiceHost host, int storeCount = 6, bool isGlobal = true)
        // {
        //     var builder = host.Register(context => new MemoryCache(storeCount)).As<ICache>();
        //     if (isGlobal) builder.SingleInstance();
        //     return host;
        // }
    }
}
