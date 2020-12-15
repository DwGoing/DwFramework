using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Linq;
using System.Collections.Generic;

using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace DwFramework.WebSocket
{
    public class WebSocketClient
    {
        public class OnConnectEventargs : EventArgs
        {

        }

        public class OnSendEventargs : EventArgs
        {
            public byte[] Data { get; }

            public OnSendEventargs(byte[] data)
            {
                Data = data;
            }
        }

        public class OnReceiveEventargs : EventArgs
        {
            public byte[] Data { get; }

            public OnReceiveEventargs(byte[] data)
            {
                Data = data;
            }
        }

        public class OnCloceEventargs : EventArgs
        {

        }

        public class OnErrorEventargs : EventArgs
        {
            public Exception Exception { get; }

            public OnErrorEventargs(Exception exception)
            {
                Exception = exception;
            }
        }

        public event Action<OnConnectEventargs> OnConnect;
        public event Action<OnCloceEventargs> OnClose;
        public event Action<OnSendEventargs> OnSend;
        public event Action<OnReceiveEventargs> OnReceive;
        public event Action<OnErrorEventargs> OnError;

        private readonly ClientWebSocket _client;
        private int _bufferSize = 4096;
        public int BufferSize
        {
            get
            {
                return _bufferSize;
            }
            set
            {
                if (value <= 0) _bufferSize = 4096;
                else _bufferSize = value;
            }
        }
        public WebSocketState State => _client.State;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebSocketClient()
        {
            _client = new ClientWebSocket();
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="header"></param>
        /// <param name="subProtocal"></param>
        /// <returns></returns>
        public async Task ConnectAsync(string uri, Dictionary<string, string> header = null, List<string> subProtocal = null)
        {
            if (header != null) foreach (var item in header) _client.Options.SetRequestHeader(item.Key, item.Value);
            if (subProtocal != null) foreach (var item in subProtocal) _client.Options.AddSubProtocol(item);
            await _client.ConnectAsync(new Uri(uri), CancellationToken.None).ContinueWith(a =>
            {
                OnConnect?.Invoke(new OnConnectEventargs() { });
                TaskManager.CreateTask(async () =>
                {
                    var buffer = new byte[_bufferSize];
                    var dataBytes = new List<byte>();
                    while (true)
                    {
                        try
                        {
                            var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            if (result.CloseStatus.HasValue) break;
                            dataBytes.AddRange(buffer.Take(result.Count));
                            if (!result.EndOfMessage) continue;
                            OnReceive?.Invoke(new OnReceiveEventargs(dataBytes.ToArray()));
                            dataBytes.Clear();
                        }
                        catch (Exception ex)
                        {
                            OnError?.Invoke(new OnErrorEventargs(ex));
                            continue;
                        }
                    }
                    OnClose?.Invoke(new OnCloceEventargs() { });
                    ClearAllEvent();
                    if (_client.State == WebSocketState.CloseReceived) await CloseAsync();
                });
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async Task SendAsync(byte[] buffer)
        {
            await _client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully) OnSend?.Invoke(new OnSendEventargs(buffer));
                else OnError?.Invoke(new OnErrorEventargs(task.Exception.InnerException));
            });
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }

        /// <summary>
        /// 清空事件列表
        /// </summary>
        private void ClearAllEvent()
        {
            if (OnConnect != null)
            {
                var actions = OnConnect.GetInvocationList();
                actions.ForEach(item => OnConnect -= (Action<OnConnectEventargs>)item);
            }
            if (OnClose != null)
            {
                var actions = OnClose.GetInvocationList();
                actions.ForEach(item => OnClose -= (Action<OnCloceEventargs>)item);
            }
            if (OnError != null)
            {
                var actions = OnError.GetInvocationList();
                actions.ForEach(item => OnError -= (Action<OnErrorEventargs>)item);
            }
            if (OnSend != null)
            {
                var actions = OnSend.GetInvocationList();
                actions.ForEach(item => OnSend -= (Action<OnSendEventargs>)item);
            }
            if (OnReceive != null)
            {
                var actions = OnReceive.GetInvocationList();
                actions.ForEach(item => OnReceive -= (Action<OnReceiveEventargs>)item);
            }
        }
    }
}
