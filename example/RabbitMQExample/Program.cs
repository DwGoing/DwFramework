using System;
using System.Threading.Tasks;
using System.Text;
using RabbitMQ.Client;
using DwFramework.Core;
using DwFramework.RabbitMQ;

namespace RabbitMQExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.ConfigureRabbitMQWithJson("Config.json");
            host.OnHostStarted += p =>
            {
                var s = p.GetRabbitMQ();
                // 创建交换机
                s.ExchangeDeclare("exchange.direct", ExchangeType.Direct);
                // 创建队列
                s.QueueDeclare("queueA", true);
                // 绑定
                s.QueueBind("queueA", "exchange.direct", "A");
                // 订阅
                s.Subscribe("queueA", true, (m, a) => Console.WriteLine(Encoding.UTF8.GetString(a.Body.ToArray())));
                // 发布消息
                s.Publish(new
                {
                    Id = 1,
                    Name = "Joy"
                }, "exchange.direct", "A");
            };
            await host.RunAsync();
        }
    }
}
