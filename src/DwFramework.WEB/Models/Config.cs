using System;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DwFramework.WEB
{
    public sealed class Config
    {
        public string ContentRoot { get; init; }
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
}