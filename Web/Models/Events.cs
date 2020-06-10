using System;

namespace DwFramework.Web
{
    #region WebSocket
    #region  服务端
    public delegate void OnWebSocketConnectHandler(WebSocketConnection connection, OnWebSocketConnectEventargs args);
    public delegate void OnWebSocketSendHandler(WebSocketConnection connection, OnWebSocketSendEventargs args);
    public delegate void OnWebSocketReceiveHandler(WebSocketConnection connection, OnWebSocketReceiveEventargs args);
    public delegate void OnWebSocketCloseHandler(WebSocketConnection connection, OnWebSocketCloceEventargs args);
    public delegate void OnWebSocketErrorHandler(WebSocketConnection connection, OnWebSocketErrorEventargs args);
    #endregion

    #region 客户端
    public delegate void OnConnectToServerHandler(OnWebSocketConnectEventargs args);
    public delegate void OnSendToServerHandler(OnWebSocketSendEventargs args);
    public delegate void OnReceiveFromServerHandler(OnWebSocketReceiveEventargs args);
    public delegate void OnCloseFromServerHandler(OnWebSocketCloceEventargs args);
    public delegate void OnErrorFromServerHandler(OnWebSocketErrorEventargs args);
    #endregion

    public class OnWebSocketConnectEventargs : EventArgs
    {

    }

    public class OnWebSocketSendEventargs : EventArgs
    {
        public string Message { get; private set; }

        public OnWebSocketSendEventargs(string msg)
        {
            Message = msg;
        }
    }

    public class OnWebSocketReceiveEventargs : EventArgs
    {
        public string Message { get; private set; }

        public OnWebSocketReceiveEventargs(string msg)
        {
            Message = msg;
        }
    }

    public class OnWebSocketCloceEventargs : EventArgs
    {

    }

    public class OnWebSocketErrorEventargs : EventArgs
    {
        public Exception Exception { get; private set; }

        public OnWebSocketErrorEventargs(Exception exception)
        {
            Exception = exception;
        }
    }
    #endregion
}
