using System;

namespace DwFramework.WebSocket
{
    public delegate void OnConnectHandler(WebSocketClient client, OnConnectEventargs args);
    public delegate void OnSendHandler(WebSocketClient client, OnSendEventargs args);
    public delegate void OnReceiveHandler(WebSocketClient client, OnReceiveEventargs args);
    public delegate void OnCloseHandler(WebSocketClient client, OnCloceEventargs args);

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
}
