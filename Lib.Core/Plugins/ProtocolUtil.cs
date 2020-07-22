using System;

using DwFramework.Core.Extensions;

namespace DwFramework.Core.Plugins
{
    public class BaseProtocol
    {
        public string Length { get; set; } // 18+N
        public string ID { get; set; } // 18
        public byte[] Data { get; set; } // N

        public BaseProtocol(byte[] data, string customNum = "0000")
        {
            ID = Generater.GenerateUUID(customNum);
            Data = data; 
        }
    }

    public static class ProtocolUtil
    {

    }
}
