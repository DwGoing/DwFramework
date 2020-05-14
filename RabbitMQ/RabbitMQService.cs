using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.RabbitMQ
{
    public class ExchangeType
    {
        public const string Direct = "direct";
        public const string Fanout = "fanout";
        public const string Headers = "headers";
        public const string Topic = "topic";
    }

    public class RabbitMQService : BaseService
    {
        public class Config
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string VirtualHost { get; set; }
        }

        private readonly Config _config;
        private ConnectionFactory _connectionFactory;
        private Dictionary<string, KeyValuePair<CancellationTokenSource, Task>> _subscribers;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="environment"></param>
        public RabbitMQService(IServiceProvider provider, IEnvironment environment) : base(provider, environment)
        {
            _config = _environment.GetConfiguration().GetConfig<Config>("RabbitMQ");
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _config.Host,
                Port = _config.Port,
                UserName = _config.UserName,
                Password = _config.Password,
                VirtualHost = _config.VirtualHost
            };
            _subscribers = new Dictionary<string, KeyValuePair<CancellationTokenSource, Task>>();
        }

        /// <summary>
        /// 创建交换机
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="type"></param>
        /// <param name="durable"></param>
        /// <param name="autoDelete"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public void ExchangeDeclare(string exchange, string type, bool durable = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
            }
        }

        /// <summary>
        /// 创建队列
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="durable"></param>
        /// <param name="exclusive"></param>
        /// <param name="autoDelete"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public void QueueDeclare(string queue, bool durable = false, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
            }
        }

        /// <summary>
        /// 队列绑定到交换机
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public void QueueBind(string queue, string exchange, string routingKey = "", IDictionary<string, object> arguments = null)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueBind(queue, exchange, routingKey, arguments);
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="basicPropertiesSetting"></param>
        public void Publish(string msg, string exchange = "", string routingKey = "", Action<IBasicProperties> basicPropertiesSetting = null)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                IBasicProperties basicProperties = null;
                if (basicPropertiesSetting != null)
                {
                    basicProperties = channel.CreateBasicProperties();
                    basicPropertiesSetting(basicProperties);
                }
                var body = Encoding.UTF8.GetBytes(msg);
                channel.BasicPublish(exchange, routingKey, basicProperties, body);
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="basicPropertiesSetting"></param>
        public void Publish(object data, string exchange = "", string routingKey = "", Action<IBasicProperties> basicPropertiesSetting = null)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                IBasicProperties basicProperties = null;
                if (basicPropertiesSetting != null)
                {
                    basicProperties = channel.CreateBasicProperties();
                    basicPropertiesSetting(basicProperties);
                }
                var body = Encoding.UTF8.GetBytes(data.ToJson());
                channel.BasicPublish(exchange, routingKey, basicProperties, body);
            }
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="autoAck"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public void Subscribe(string queue, bool autoAck, Action<IModel, BasicDeliverEventArgs> handler)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            _subscribers[queue] = new KeyValuePair<CancellationTokenSource, Task>(tokenSource, Task.Run(() =>
            {
                using (var connection = _connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue, autoAck, consumer);
                    consumer.Received += (sender, args) =>
                    {
                        handler(channel, args);
                    };
                    while (!token.IsCancellationRequested) Thread.Sleep(1);
                }
            }, token));
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public void Unsubscribe(string queue)
        {
            if (!_subscribers.ContainsKey(queue))
                throw new Exception("该队列不存在");
            _subscribers[queue].Key.Cancel();
            _subscribers.Remove(queue);
        }

    }
}
