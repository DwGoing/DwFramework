using System;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DwFramework.Web
{
    public sealed class Config
    {
        public sealed class Socket
        {
            public Dictionary<string, string> Listens { get; init; }
            public int BufferSize { get; init; } = 1024 * 4;
            public int BackLog { get; init; } = 100;
            [JsonConverter(typeof(AddressFamily))]
            public AddressFamily AddressFamily { get; init; }
            [JsonConverter(typeof(SocketType))]
            public SocketType SocketType { get; init; }
            [JsonConverter(typeof(ProtocolType))]
            public ProtocolType ProtocolType { get; init; }
        }

        public sealed class Web
        {
            public bool EnableHttp { get; init; } = false;
            public bool EnableRpc { get; init; } = false;
            public bool EnableWebSocket { get; init; } = false;
            public string ContentRoot { get; init; }
            public Dictionary<string, string> Listens { get; init; }
            public int BufferSize { get; init; } = 1024 * 4;
        }
    }
}