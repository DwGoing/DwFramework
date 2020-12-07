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
                        //mq.Subscribe("queue.indexservice.datapush.DiagnosisAndTreatment", false, (m, a) =>
                        //{
                        //    Console.WriteLine("DiagnosisAndTreatment");
                        //    Thread.Sleep(200);
                        //    m.BasicAck(a.DeliveryTag, false);
                        //}, 2, 5);
                        //mq.Subscribe("queue.indexservice.datapush.FamilyMedicine", false, (m, a) =>
                        //{
                        //    Console.WriteLine("FamilyMedicine");
                        //    Thread.Sleep(200);
                        //    m.BasicAck(a.DeliveryTag, false);
                        //}, 2, 5);
                        mq.Subscribe("queue.indexservice.datapush.PublicHealth", false, (m, a) =>
                        {
                            var i = new Random().Next(50, 200);
                            Console.WriteLine(i + "ms");
                            Thread.Sleep(i);
                            m.BasicAck(a.DeliveryTag, false);
                        }, 3, 10);
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
