using Google.Protobuf;

namespace DwFramework.Rpc
{
    public partial class Bytes
    {
        public byte[] RawValue { get => Value.ToByteArray(); }

        public Bytes(byte[] value)
        {
            Value = ByteString.CopyFrom(value);
        }
    }
}
