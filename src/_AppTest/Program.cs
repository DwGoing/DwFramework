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
using DwFramework.RabbitMQ;
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
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterLog();
                host.AddJsonConfig("RabbitMQ.json");
                host.RegisterRabbitMQService();
                host.OnInitialized += p =>
                {
                    try
                    {
                        var mq = p.GetRabbitMQService();
                        mq.Subscribe("queue.indexservice.datapush.DiagnosisAndTreatment", true, (m, a) =>
                        {
                            Console.WriteLine(System.Text.Encoding.UTF8.GetString(a.Body.ToArray()));
                        });
                        mq.Publish(new { A = 111 }, "exchange.indexservice.direct", "datapush.DiagnosisAndTreatment", System.Text.Encoding.UTF8);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
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
