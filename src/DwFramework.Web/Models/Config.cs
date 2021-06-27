using System;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DwFramework.Web
{
    public sealed class Config
    {
        public sealed class Listen
        {
            [JsonConverter(typeof(Scheme))]
            public Scheme Scheme { get; init; }
            public string Ip { get; init; }
            public int Port { get; init; }
            public string Cert { get; init; }
            public string Password { get; init; }
        }

        public sealed class Socket
        {
            public Listen Listen { get; init; }
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
            public string ContentRoot { get; init; }
            public List<Listen> Listens { get; init; } = new();
            public int BufferSize { get; init; } = 1024 * 4;
        }
    }
}