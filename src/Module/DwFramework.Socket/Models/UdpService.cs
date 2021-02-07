using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.Socket
{
    public sealed class UdpService : ConfigableService
    {
        public sealed class Config
        {
            public string Listen { get; init; }
            public string Limit { get; init; }
            public int BufferSize { get; init; } = 1024 * 4;
        }

        public class OnSendEventargs : EventArgs
        {
            public IPEndPoint Remote { get; init; }
            public byte[] Data { get; init; }
        }

        public class OnReceiveEventargs : EventArgs
        {
            public IPEndPoint Remote { get; init; }
            public byte[] Data { get; init; }
        }

        public class OnErrorEventArgs : EventArgs
        {
            public IPEndPoint Remote { get; init; }
            public Exception Exception { get; init; }
        }

        private readonly ILogger<UdpService> _logger;
        private Config _config;
        private System.Net.Sockets.Socket _server;
        private byte[] _buffer;

        public event Action<OnSendEventargs> OnSend;
        public event Action<OnReceiveEventargs> OnReceive;
        public event Action<OnErrorEventArgs> OnError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public UdpService(ILogger<UdpService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public void ReadConfig(string path = null, string key = null)
        {
            try
            {
                _config = ReadConfig<Config>(path, key);
                if (_config == null) throw new Exception("未读取到UDP配置");
            }
            catch (Exception ex)
            {
                _ = _logger?.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            if (_config.Listen == null) throw new Exception("缺少Listen配置");
            _server = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var ipAndPort = _config.Listen.Split(":");
            _server.Bind(new IPEndPoint(string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])));
            _buffer = new byte[_config.BufferSize];
            _ = BeginReceiveAsync();
            await _logger?.LogInformationAsync($"Udp服务正在监听:{_config.Listen}");
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _server.Dispose();
        }

        /// <summary>
        /// 开始接收消息
        /// </summary>
        /// <returns></returns>
        private async Task BeginReceiveAsync()
        {
            IPEndPoint remote = null;
            try
            {
                var limit = new IPEndPoint(IPAddress.Any, 0);
                var result = await _server.ReceiveFromAsync(_buffer, SocketFlags.None, limit);
                var len = result.ReceivedBytes;
                remote = (IPEndPoint)result.RemoteEndPoint;
                if (len > 0)
                {
                    var data = new byte[len];
                    Array.Copy(_buffer, data, len);
                    OnReceive?.Invoke(new OnReceiveEventargs()
                    {
                        Remote = remote,
                        Data = data
                    });
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new OnErrorEventArgs()
                {
                    Remote = remote,
                    Exception = ex
                });
            }
            finally
            {
                await BeginReceiveAsync();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> SendAsync(IPEndPoint remote, byte[] data)
        {
            try
            {
                var len = await _server.SendToAsync(new ArraySegment<byte>(data), SocketFlags.None, remote);
                OnSend?.Invoke(new OnSendEventargs()
                {
                    Remote = remote,
                    Data = data
                });
                return len;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new OnErrorEventArgs()
                {
                    Remote = remote,
                    Exception = ex
                });
                return 0;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> SendAsync(string ip, int port, byte[] data)
        {
            var remote = new IPEndPoint(IPAddress.Parse(ip), port);
            return await SendAsync(remote, data);
        }
    }
}
