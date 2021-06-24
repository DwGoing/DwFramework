using System;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;

namespace DwFramework.Web
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

    public class OnReceiveEventArgs : EventArgs
    {
        public byte[] Data { get; init; }
    }

    public class OnErrorEventArgs : EventArgs
    {
        public Exception Exception { get; init; }
    }
}