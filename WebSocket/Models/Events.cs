using System;

namespace DwFramework.WebSocket
{
    #region  服务端
    public delegate void OnConnectHandler(WebSocketConnection connection, OnConnectEventargs args);
    public delegate void OnSendHandler(WebSocketConnection connection, OnSendEventargs args);
    public delegate void OnReceiveHandler(WebSocketConnection connection, OnReceiveEventargs args);
    public delegate void OnCloseHandler(WebSocketConnection connection, OnCloceEventargs args);
    public delegate void OnErrorHandler(WebSocketConnection connection, OnErrorEventargs args);
    #endregion

    #region 客户端
    public delegate void OnConnectToServerHandler(OnConnectEventargs args);
    public delegate void OnSendToServerHandler(OnSendEventargs args);
    public delegate void OnReceiveFromServerHandler(OnReceiveEventargs args);
    public delegate void OnCloseFromServerHandler(OnCloceEventargs args);
    public delegate void OnErrorFromServerHandler(OnErrorEventargs args);
    #endregion

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
}
