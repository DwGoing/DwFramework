using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;

using DwFramework.Core.Helper;

namespace DwFramework.Web
{
    public class WebSocketClient
    {
        public class OnConnectEventargs : EventArgs
        {

        }

        public class OnSendEventargs : EventArgs
        {
            public string Message { get; private set; }

            public OnSendEventargs(string msg)
            {
                Message = msg;
            }
        }

        public class OnReceiveEventargs : EventArgs
        {
            public string Message { get; private set; }

            public OnReceiveEventargs(string msg)
            {
                Message = msg;
            }
        }

        public class OnCloceEventargs : EventArgs
        {

        }

        public class OnErrorEventargs : EventArgs
        {
            public Exception Exception { get; private set; }

            public OnErrorEventargs(Exception exception)
            {
                Exception = exception;
            }
        }

        public event Action<OnConnectEventargs> OnConnect;
        public event Action<OnSendEventargs> OnSend;
        public event Action<OnReceiveEventargs> OnReceive;
        public event Action<OnCloceEventargs> OnClose;
        public event Action<OnErrorEventargs> OnError;

        private ClientWebSocket _client;
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
        /// <returns></returns>
        public Task ConnectAsync(string uri)
        {
            return _client.ConnectAsync(new Uri(uri), CancellationToken.None).ContinueWith(a =>
            {
                OnConnect?.Invoke(new OnConnectEventargs() { });
                TaskManager.CreateTask(async () =>
                {
                    var buffer = new byte[_bufferSize];
                    var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    while (!result.CloseStatus.HasValue)
                    {
                        try
                        {
                            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            OnReceive?.Invoke(new OnReceiveEventargs(msg));
                        }
                        catch (Exception ex)
                        {
                            OnError?.Invoke(new OnErrorEventargs(ex));
                        }
                        finally
                        {
                            buffer = new byte[_bufferSize];
                            result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        }
                    }
                    OnClose?.Invoke(new OnCloceEventargs() { });
                });
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task SendAsync(byte[] buffer)
        {
            return _client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None)
                .ContinueWith(a => OnSend?.Invoke(new OnSendEventargs(Encoding.UTF8.GetString(buffer)) { }));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task SendAsync(string msg)
        {
            return SendAsync(Encoding.UTF8.GetBytes(msg));
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync()
        {
            return _client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }
    }
}
