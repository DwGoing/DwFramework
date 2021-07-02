using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using DwFramework.Core.Encrypt;

namespace DwFramework.Web
{
    public class WebSocketConnection
    {
        public string ID { get; init; }
        public bool IsClose { get; private set; } = false;

        private readonly WebSocket _webSocket;
        private readonly byte[] _buffer;
        private readonly List<byte> _dataBytes = new();
        private readonly AutoResetEvent _resetEvent;

        public Action<WebSocketConnection, OnCloceEventArgs> OnClose { get; init; }
        public Action<WebSocketConnection, OnSendEventArgs> OnSend { get; init; }
        public Action<WebSocketConnection, OnReceiveEventArgs> OnReceive { get; init; }
        public Action<WebSocketConnection, OnErrorEventArgs> OnError { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="bufferSize"></param>
        /// <param name="resetEvent"></param>
        public WebSocketConnection(WebSocket webSocket, int bufferSize, out AutoResetEvent resetEvent)
        {
            ID = MD5.Encrypt(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
            _webSocket = webSocket;
            _buffer = new byte[bufferSize > 0 ? bufferSize : 4096];
            _resetEvent = new AutoResetEvent(false);
            resetEvent = _resetEvent;
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <returns></returns>
        public async Task BeginReceiveAsync()
        {
            try
            {
                if (IsClose) return;
                if (_webSocket.State != WebSocketState.Open) throw new WebSocketException("连接状态异常");
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(_buffer), CancellationToken.None);
                if (result.CloseStatus.HasValue)
                {
                    if (_webSocket.State == WebSocketState.CloseReceived && !IsClose)
                        await CloseAsync(result.CloseStatus.Value);
                    return;
                }
                _dataBytes.AddRange(_buffer.Take(result.Count));
                if (result.EndOfMessage)
                {
                    OnReceive?.Invoke(this, new OnReceiveEventArgs() { Data = _dataBytes.ToArray() });
                    _dataBytes.Clear();
                }
                await BeginReceiveAsync();
            }
            catch (WebSocketException ex)
            {
                switch (ex.ErrorCode)
                {
                    // TODO
                    default:
                        OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
                        await CloseAsync(WebSocketCloseStatus.InternalServerError);
                        break;
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
                await BeginReceiveAsync();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async Task SendAsync(byte[] buffer)
        {
            try
            {
                if (_webSocket.State != WebSocketState.Open) throw new Exception("连接状态异常");
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                OnSend?.Invoke(this, new OnSendEventArgs() { Data = buffer });
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="closeStatus"></param>
        /// <returns></returns>
        public async Task CloseAsync(WebSocketCloseStatus closeStatus)
        {
            if (IsClose) return;
            await _webSocket.CloseOutputAsync(closeStatus, null, CancellationToken.None);
            _webSocket.Abort();
            _webSocket.Dispose();
            IsClose = true;
            OnClose?.Invoke(this, new OnCloceEventArgs() { });
            _resetEvent.Set();
        }
    }
}
