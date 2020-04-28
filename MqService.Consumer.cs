// -----------------------------------------------------------------------
//  <copyright file="MqService.cs" company="禾卓软件科技有限公司">
//      Copyright (c) 2020 禾卓软件科技有限公司. All rights reserved.
//  </copyright>
//  <last-editor>TANMINGCHAO - 2020/4/28 16:29:38 </last-editor>
// -----------------------------------------------------------------------

namespace Infrastructure.Mq.RabbitMQ
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="MqService" />.
    /// </summary>
    public partial class MqService
    {
        /// <summary>
        /// Defines the _object.
        /// </summary>
        private static readonly Object _object = new object();

        /// <summary>
        /// The Subscribe.
        /// </summary>
        /// <param name="queueName">The queueName<see cref="string"/>.</param>
        /// <param name="func">The func<see cref="Func{string, bool}"/>.</param>
        public void Subscribe(string queueName, Func<string, bool> func)
        {
            this.Subscribe(() => new MqSettings(queueName), func);
        }

        /// <summary>
        /// The Subscribe.
        /// </summary>
        /// <param name="settings">The settings<see cref="Func{MqSettings}"/>.</param>
        /// <param name="func">The func<see cref="Func{string, bool}"/>.</param>
        public void Subscribe(Func<MqSettings> settings, Func<string, bool> func)
        {
            MqSettings config = settings();
            if (string.IsNullOrWhiteSpace(config.QueueName))
                throw new Exception($"{nameof(config.QueueName)}不可为空");

            try
            {
                Task.Run(() =>
                {
                    if (_channel == null || _channel.IsClosed)
                    {
                        _channel = _connection.CreateModel();

                        //设置交换器及类型
                        _channel.ExchangeDeclare(
                            config.ExChangeName,
                            config.ExChangeType,
                            config.Durable,
                            config.AutoDelete,
                            config.Arguments);

                        //声明队列
                        _channel.QueueDeclare(
                            queue: config.QueueName,//队列名称
                            durable: config.Durable,//是否持久化 true
                            exclusive: config.Exclusive,//是否排它性 false
                            autoDelete: config.AutoDelete,//是否自动删除 false
                            arguments: config.Arguments);

                    }

                    //此处临时如此处理，后面有特殊需求在做扩展，这里是一次从队列拿一条
                    _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);//告诉broker同一时间只处理一个消息

                    var consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += (currentConsumer, ea) =>
                    {
                        try
                        {
                            lock (_object)
                            {
                                var body = ea.Body;
                                var message = Encoding.UTF8.GetString(body);
                                _logger.LogInformation($"接收到{config.QueueName}消息: {message} )");

                                bool blnResult = func(message);

                                if (blnResult)
                                {
                                    _logger.LogInformation($"MQConsumerService.Subscribor 调用委托执行完成");

                                    if (!config.AutoAck)
                                    {
                                        _channel.BasicAck(ea.DeliveryTag, false);//手动应答
                                        IBasicProperties props = ea.BasicProperties;//传来的属性
                                        _channel.BasicPublish(ea.Exchange, ea.RoutingKey, props, ea.Body);
                                    }
                                    else
                                        _channel.BasicAck(ea.DeliveryTag, true);
                                }
                                else
                                {
                                    _logger.LogWarning($"消费端回调函执行结果为：{blnResult}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "MQConsumerService.Subscribor 执行失败");
                            if (!config.AutoAck)
                            {
                                /*
                                 此种方式存在缺陷，方便其他人学习，所以我写在这，会导致一个很严重的结果就是消息在消费失败之后回退队列时，会一直处于队列末尾，
                                 这样会循环往复的被处理，如果一直失败，其他消息也失败，长期会导致消息堆积，大量堆积。
                                 */
                                //channel.BasicReject(ea.DeliveryTag, command.Requeue);
                                //换成以下方式（将错误或消费失败消息推送到队列尾部）
                                _channel.BasicAck(ea.DeliveryTag, false);//手动应答
                                IBasicProperties props = ea.BasicProperties;//传来的属性
                                _channel.BasicPublish(ea.Exchange, ea.RoutingKey, props, ea.Body);

                                _logger.LogError(ex, "MQConsumerService.Subscribor.catch 已将消息重新推到队列（头部） ");
                            }
                        }
                    };

                    _channel.BasicConsume(queue: config.QueueName,
                                        autoAck: config.AutoAck,
                                        consumer: consumer);

                    _logger.LogInformation($"StartConsumer Successed");
                    Thread.Sleep(Timeout.Infinite);
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, $"{nameof(MqService)}.Consumer 出现异常");
            }
        }
    }
}
