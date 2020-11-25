using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;
using DwFramework.Socket;
using DwFramework.TaskSchedule;
using DwFramework.WebAPI;
using DwFramework.WebSocket;

namespace _AppTest
{
    class Program
    {
        public class Config
        {
            public Dictionary<string, RawDataStruct> RawDataStructs { get; set; }
        }

        public enum Method
        {
            Unknow,
            View,
            Request,
            Push
        }

        public class RawDataStruct
        {
            public Method Method { get; set; }
            public Dictionary<string, Property> Properties { get; set; }
        }

        public class Property
        {
            public string Map { get; set; }
            public bool IsEnum { get; set; }
            public string EnumName { get; set; }
        }

        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterLog();
                host.AddJsonConfig("Config.json");
                host.OnInitialized += p =>
                {
                    var config = p.GetService<DwFramework.Core.Environment>().GetConfiguration().GetConfig<Config>();
                    var a = JsonDocument.Parse("{\"A\":{\"B\":10}}");
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
