using System;
using ProtoBuf;

namespace DwFramework.RPC.Plugins
{
    [ProtoContract]
    public sealed class JoinResponse
    {
        [ProtoMember(1)]
        public string RemoteId { get; set; }
    }

    [ProtoContract]
    public sealed class SyncDataRequest
    {
        [ProtoMember(1)]
        public int Type { get; set; }
        [ProtoMember(2)]
        public string Hex { get; set; }
    }
}