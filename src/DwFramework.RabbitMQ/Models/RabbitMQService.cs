using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using DwFramework.Core;

namespace DwFramework.RabbitMQ
{
    public sealed class RabbitMQService
    {
        private readonly Config _config;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _publishConnection;
        private IConnection _subscribeConnection;
        private readonly Dictionary<string, EventingBasicConsumer[]> _subscribers = new Dictionary<string, EventingBasicConsumer[]>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public RabbitMQService(IConfiguration configuration)
        {
            _config = configuration.Get<Config>();
            if (_config == null) throw new NotFoundException("缺少RabbitMQ配置");
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
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        public IConnection CreateConnection() => _connectionFactory.CreateConnection();

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
        /// 获取队列中消息数量
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public uint GetMessageCount(string queue)
        {
            using var conn = _connectionFactory.CreateConnection();
            using var channel = conn.CreateModel();
            return channel.MessageCount(queue);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="basicPropertiesSetting"></param>
        /// <param name="returnAction"></param>
        public void Publish(byte[] data, string exchange = "", string routingKey = "", Action<IBasicProperties> basicPropertiesSetting = null, Action<BasicReturnEventArgs> returnAction = null)
        {
            _publishConnection ??= _connectionFactory.CreateConnection();
            using var channel = _publishConnection.CreateModel();
            IBasicProperties basicProperties = null;
            if (basicPropertiesSetting != null)
            {
                basicProperties = channel.CreateBasicProperties();
                basicPropertiesSetting(basicProperties);
            }
            if (returnAction != null) channel.BasicReturn += (sender, args) => returnAction?.Invoke(args);
            channel.BasicPublish(exchange, routingKey, basicProperties, data);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="basicPropertiesSetting"></param>
        /// <param name="returnAction"></param>
        /// <returns></returns>
        public Task PublishAsync(byte[] data, string exchange = "", string routingKey = "", Action<IBasicProperties> basicPropertiesSetting = null, Action<BasicReturnEventArgs> returnAction = null)
            => Task.Factory.StartNew(() => Publish(data, exchange, routingKey, basicPropertiesSetting, returnAction));

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="encoding"></param>
        /// <param name="basicPropertiesSetting"></param>
        /// <param name="returnAction"></param>
        public void Publish<T>(T data, string exchange = "", string routingKey = "", Encoding encoding = null, Action<IBasicProperties> basicPropertiesSetting = null, Action<BasicReturnEventArgs> returnAction = null)
        {
            var body = data.ToJsonBytes(encoding);
            Publish(body, exchange, routingKey, basicPropertiesSetting, returnAction);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="encoding"></param>
        /// <param name="basicPropertiesSetting"></param>
        /// <param name="returnAction"></param>
        /// <returns></returns>
        public Task PublishAsync<T>(T data, string exchange = "", string routingKey = "", Encoding encoding = null, Action<IBasicProperties> basicPropertiesSetting = null, Action<BasicReturnEventArgs> returnAction = null)
            => Task.Factory.StartNew(() => Publish(data, exchange, routingKey, encoding, basicPropertiesSetting, returnAction));

        /// <summary>
        /// 发布消息并等待Ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="basicPropertiesSetting"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="returnAction"></param>
        /// <returns></returns>
        public bool PublishWaitForAck<T>(T data, string exchange = "", string routingKey = "", Encoding encoding = null, Action<IBasicProperties> basicPropertiesSetting = null, int timeoutSeconds = 0, Action<BasicReturnEventArgs> returnAction = null)
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
            var body = data.ToJsonBytes(encoding);
            if (returnAction != null) channel.BasicReturn += (sender, args) => returnAction?.Invoke(args);
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
        public Task<bool> PublishWaitForAckAsync<T>(T data, string exchange = "", string routingKey = "", Encoding encoding = null, Action<IBasicProperties> basicPropertiesSetting = null, int timeoutSeconds = 0, Action<BasicReturnEventArgs> returnAction = null)
            => Task.Factory.StartNew(() => PublishWaitForAck(data, exchange, routingKey, encoding, basicPropertiesSetting, timeoutSeconds, returnAction));

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
                consumer.Received += (sender, args) => handler?.Invoke(channel, args);
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
