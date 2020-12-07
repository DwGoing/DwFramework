﻿using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

namespace DwFramework.RabbitMQ
{
    public sealed class ExchangeType
    {
        public const string Direct = "direct";
        public const string Fanout = "fanout";
        public const string Headers = "headers";
        public const string Topic = "topic";
    }

    public sealed class Config
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; } = "/";
    }

    public sealed class RabbitMQService
    {
        private readonly Config _config;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _publishConnection;
        private IConnection _subscribeConnection;
        private readonly Dictionary<string, EventingBasicConsumer[]> _subscribers;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        public RabbitMQService(string configKey = null, string configPath = null)
        {
            _config = ServiceHost.Environment.GetConfiguration<Config>(configKey, configPath);
            if (_config == null) throw new Exception("未读取到RabbitMQ配置");
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _config.Host,
                Port = _config.Port,
                UserName = _config.UserName,
                Password = _config.Password,
                VirtualHost = _config.VirtualHost
            };
            _publishConnection = _connectionFactory.CreateConnection();
            _subscribeConnection = _connectionFactory.CreateConnection();
            _subscribers = new Dictionary<string, EventingBasicConsumer[]>();
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
            _publishConnection ??= _connectionFactory.CreateConnection();
            using var channel = _publishConnection.CreateModel();
            channel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
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
            _publishConnection ??= _connectionFactory.CreateConnection();
            using var channel = _publishConnection.CreateModel();
            channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
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
            _publishConnection ??= _connectionFactory.CreateConnection();
            using var channel = _publishConnection.CreateModel();
            channel.QueueBind(queue, exchange, routingKey, arguments);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="encoding"></param>
        /// <param name="basicPropertiesSetting"></param>
        /// <param name="returnAction"></param>
        public void Publish(object data, string exchange = "", string routingKey = "", Encoding encoding = null, Action<IBasicProperties> basicPropertiesSetting = null, Action<BasicReturnEventArgs> returnAction = null)
        {
            _publishConnection ??= _connectionFactory.CreateConnection();
            using var channel = _publishConnection.CreateModel();
            IBasicProperties basicProperties = null;
            if (basicPropertiesSetting != null)
            {
                basicProperties = channel.CreateBasicProperties();
                basicPropertiesSetting(basicProperties);
            }
            var body = data.ToBytes(encoding);
            if (returnAction != null) channel.BasicReturn += (sender, args) => returnAction(args);
            channel.BasicPublish(exchange, routingKey, basicProperties, body);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="encoding"></param>
        /// <param name="basicPropertiesSetting"></param>
        /// <param name="returnAction"></param>
        /// <returns></returns>
        public Task PublishAsync(object data, string exchange = "", string routingKey = "", Encoding encoding = null, Action<IBasicProperties> basicPropertiesSetting = null, Action<BasicReturnEventArgs> returnAction = null)
        {
            return TaskManager.CreateTask(() => Publish(data, exchange, routingKey, encoding, basicPropertiesSetting, returnAction));
        }

        /// <summary>
        /// 发布消息并等待Ack
        /// </summary>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="basicPropertiesSetting"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="returnAction"></param>
        /// <returns></returns>
        public bool PublishWaitForAck(object data, string exchange = "", string routingKey = "", Encoding encoding = null, Action<IBasicProperties> basicPropertiesSetting = null, int timeoutSeconds = 0, Action<BasicReturnEventArgs> returnAction = null)
        {
            _publishConnection ??= _connectionFactory.CreateConnection();
            using var channel = _publishConnection.CreateModel();
            channel.ConfirmSelect();
            IBasicProperties basicProperties = null;
            if (basicPropertiesSetting != null)
            {
                basicProperties = channel.CreateBasicProperties();
                basicPropertiesSetting(basicProperties);
            }
            var body = data.ToBytes(encoding);
            if (returnAction != null) channel.BasicReturn += (sender, args) => returnAction(args);
            channel.BasicPublish(exchange, routingKey, basicProperties, body);
            if (timeoutSeconds >= 0) return channel.WaitForConfirms(TimeSpan.FromSeconds(timeoutSeconds));
            else return channel.WaitForConfirms();
        }

        /// <summary>
        /// 发布消息并等待Ack
        /// </summary>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="basicPropertiesSetting"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="returnAction"></param>
        /// <returns></returns>
        public Task<bool> PublishWaitForAckAsync(object data, string exchange = "", string routingKey = "", Encoding encoding = null, Action<IBasicProperties> basicPropertiesSetting = null, int timeoutSeconds = 0, Action<BasicReturnEventArgs> returnAction = null)
        {
            return TaskManager.CreateTask(() => PublishWaitForAck(data, exchange, routingKey, encoding, basicPropertiesSetting, timeoutSeconds, returnAction));
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="autoAck"></param>
        /// <param name="handler"></param>
        /// <param name="consumerCount"></param>
        /// <param name="qosCount"></param>
        public void Subscribe(string queue, bool autoAck, Action<IModel, BasicDeliverEventArgs> handler, int consumerCount = 1, ushort qosCount = 100)
        {
            _subscribeConnection ??= _connectionFactory.CreateConnection();
            _subscribers[queue] = new EventingBasicConsumer[consumerCount];
            for (var i = 0; i < consumerCount; i++)
            {
                var channel = _subscribeConnection.CreateModel();
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, args) => handler(channel, args);
                channel.BasicQos(0, qosCount, true);
                channel.BasicConsume(queue, autoAck, consumer);
                _subscribers[queue][i] = consumer;
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public void Unsubscribe(string queue)
        {
            if (!_subscribers.ContainsKey(queue)) return;
            _subscribers[queue].ForEach(item => item.Model.Abort());
            _subscribers.Remove(queue);
        }

    }
}
