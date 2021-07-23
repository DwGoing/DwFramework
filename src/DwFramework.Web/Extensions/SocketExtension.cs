using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.Web.Socket
{
    public static class SocketExtension
    {
        /// <summary>
        /// 配置Socket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSocket(this ServiceHost host, IConfiguration configuration, string path = null)
        {
            var config = configuration.ParseConfiguration<Config.Socket>(path);
            if (config == null) throw new NotFoundException("未读取到Socket配置");
            switch (config.ProtocolType)
            {
                case ProtocolType.Tcp:
                    var tcpService = new TcpService(configuration.GetConfiguration(path));
                    host.ConfigureContainer(builder => builder.RegisterInstance(tcpService).SingleInstance());
                    break;
                case ProtocolType.Udp:
                    var udpService = new UdpService(configuration.GetConfiguration(path));
                    host.ConfigureContainer(builder => builder.RegisterInstance(udpService).SingleInstance());
                    break;
                default:
                    throw new Exception("未定义的协议类型");
            }
            return host;
        }

        /// <summary>
        /// 配置Socket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSocketWithJson(this ServiceHost host, string file, string path = null)
            => host.ConfigureSocket(new ConfigurationBuilder().AddJsonFile(file).Build(), path);

        /// <summary>
        /// 配置Socket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSocketWithJson(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureSocket(new ConfigurationBuilder().AddJsonStream(stream).Build(), path);

        /// <summary>
        /// 配置Socket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSocketWithXml(this ServiceHost host, string file, string path = null)
            => host.ConfigureSocket(new ConfigurationBuilder().AddXmlFile(file).Build(), path);

        /// <summary>
        /// 配置Socket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSocketWithXml<T>(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureSocket(new ConfigurationBuilder().AddXmlStream(stream).Build(), path);

        /// <summary>
        /// 启用keep-alive
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="intervalStart"></param>
        /// <param name="retryInterval"></param>
        public static void EnableKeepAlive(this System.Net.Sockets.Socket socket, uint keepAliveTime, uint interval)
        {
            var size = sizeof(uint);
            var inArray = new byte[size * 3];
            Array.Copy(BitConverter.GetBytes(1u), 0, inArray, 0, size);
            Array.Copy(BitConverter.GetBytes(keepAliveTime), 0, inArray, size, size);
            Array.Copy(BitConverter.GetBytes(interval), 0, inArray, size * 2, size);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inArray);
        }

        /// <summary>
        /// 获取Tcp服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="TcpService"></typeparam>
        /// <returns></returns>
        public static TcpService GetTcp(this IServiceProvider provider) => provider.GetService<TcpService>();

        /// <summary>
        /// 获取Udp服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="UdpService"></typeparam>
        /// <returns></returns>
        public static UdpService GetUdp(this IServiceProvider provider) => provider.GetService<UdpService>();
    }
}