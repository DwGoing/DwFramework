using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.RabbitMQ;

using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Autofac;

namespace _AppTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterRabbitMQService("RabbitMQ");
                host.OnInitialized += async p =>
                {
                    var s = p.GetRabbitMQService();
                    s.ExchangeDeclare("EX", ExchangeType.Direct);
                    s.QueueDeclare("Q1");
                    s.QueueBind("Q1", "EX", "1");
                    s.Subscribe("Q1", true, (a, b) => Console.WriteLine(Encoding.UTF8.GetString(b.Body.ToArray())));
                    s.Publish("Hello", "EX", "1", Encoding.UTF8);
                    await Task.Delay(5000);
                    p.ReconfigRabbitMQService("RabbitMQ");
                    s.ExchangeDeclare("EX1", ExchangeType.Direct);
                    s.QueueDeclare("Q2");
                    s.QueueBind("Q2", "EX1", "2");
                    s.Subscribe("Q2", true, (a, b) => Console.WriteLine(Encoding.UTF8.GetString(b.Body.ToArray())));
                    s.Publish("World", "EX1", "2", Encoding.UTF8);
                };
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}