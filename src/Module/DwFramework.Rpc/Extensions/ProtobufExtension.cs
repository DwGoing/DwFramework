using Google.Protobuf;

namespace DwFramework.Rpc
{
    public partial class Bytes
    {
        public byte[] RawValue { get => Value.ToByteArray(); }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value"></param>
        public Bytes(byte[] value)
        {
            Value = ByteString.CopyFrom(value);
        }
    }
}
