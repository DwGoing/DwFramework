using System;
using System.Collections.Generic;

namespace DwFramework.WEB
{
    public sealed class WebApiConfig
    {
        public string ContentRoot { get; set; }
        public Dictionary<string, string> Listens { get; set; }
    }
}