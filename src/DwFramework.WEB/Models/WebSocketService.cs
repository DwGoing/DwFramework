using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using DwFramework.Core;

namespace DwFramework.WEB
{
    public sealed class WebSocketService
    {
        public class OnConnectEventArgs : EventArgs
        {
            public IHeaderDictionary Header { get; init; }
        }

        public class OnCloceEventArgs : EventArgs
        {
            public WebSocketCloseStatus? CloseStatus { get; init; }
        }

        public class OnSendEventArgs : EventArgs
        {
            public byte[] Data { get; init; }
        }

        public class OnReceiveEventargs : EventArgs
        {
            public byte[] Data { get; init; }
        }

        public class OnErrorEventArgs : EventArgs
        {
            public Exception Exception { get; init; }
        }

        private Config _config;
        private readonly Dictionary<string, WebSocketConnection> _connections = new Dictionary<string, WebSocketConnection>();
        private CancellationTokenSource _cancellationTokenSource;
        private event Action<IServiceCollection> _onConfigureServices;

        public event Action<WebSocketConnection, OnConnectEventArgs> OnConnect;
        public event Action<WebSocketConnection, OnCloceEventArgs> OnClose;
        public event Action<WebSocketConnection, OnSendEventArgs> OnSend;
        public event Action<WebSocketConnection, OnReceiveEventargs> OnReceive;
        public event Action<WebSocketConnection, OnErrorEventArgs> OnError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        public WebSocketService(Config config)
        {
            _config = config;
            if (_config == null) throw new Exception("未读取到WebSocket配置");
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WebSocketConnection GetSocketConnection(string id)
        {
            if (!_connections.ContainsKey(id)) return null;
            return _connections[id];
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void BroadCast(byte[] data)
        {
            _connections.Values.ForEach(async item => await item.SendAsync(data), (connection, ex) =>
            {
                OnError?.Invoke(connection, new OnErrorEventArgs() { Exception = ex });
            });
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        public void CloseAll()
        {
            _connections.Values.ForEach(async item => await item.CloseAsync(WebSocketCloseStatus.NormalClosure), (connection, ex) =>
            {
                OnError?.Invoke(connection, new OnErrorEventArgs() { Exception = ex });
            });
        }
    }
}
