using System;
using System.Collections.Generic;

namespace DwFramework.RPC
{
    public sealed class Config
    {
        public string ContentRoot { get; init; }
        public Dictionary<string, string> Listens { get; init; }
    }
}