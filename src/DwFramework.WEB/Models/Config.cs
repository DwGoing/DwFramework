using System;
using System.Collections.Generic;

namespace DwFramework.WEB
{
    public sealed class Config
    {
        public string ContentRoot { get; set; }
        public Dictionary<string, string> Listens { get; set; }
    }
}