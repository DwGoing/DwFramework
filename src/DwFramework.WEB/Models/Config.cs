using System;
using System.Collections.Generic;

namespace DwFramework.WEB
{
    public sealed class Config
    {
        public string ContentRoot { get; init; }
        public Dictionary<string, string> Listens { get; init; }
        public int BufferSize { get; init; } = 1024 * 4;
    }
}