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

    /// <summary>
    /// Defines the <see cref="MqService" />.
    /// </summary>
    public partial class MqService
    {
        /// <summary>
        /// The Publish.
        /// </summary>
        /// <param name="queueName">The queueName<see cref="string"/>.</param>
        /// <param name="jsonBody">The jsonBody<see cref="Func{string}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Publish(string queueName, Func<string> jsonBody)
        {
            return this.Publish<string>(() => new MqSettings(queueName), jsonBody);
        }

        /// <summary>
        /// The Publish.
        /// </summary>
        /// <param name="produceConfig">The produceConfig<see cref="Func{MqSettings}"/>.</param>
        /// <param name="jsonBody">The jsonBody<see cref="Func{string}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Publish(Func<MqSettings> produceConfig, Func<string> jsonBody)
        {
            return this.Publish<string>(produceConfig, jsonBody);
        }

        /// <summary>
        /// The Publish.
        /// </summary>
        /// <typeparam name="TObject">.</typeparam>
        /// <param name="produceConfig">The produceConfig<see cref="Func{MqSettings}"/>.</param>
        /// <param name="jsonBody">The jsonBody<see cref="Func{string}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Publish<TObject>(Func<MqSettings> produceConfig, Func<string> jsonBody)
        {
            MqSettings config = produceConfig();
            if (string.IsNullOrWhiteSpace(config.QueueName))
                throw new Exception($"{nameof(config.QueueName)}不可为空");
            try
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
                    //绑定消息队列
                    _channel.QueueBind(
                        queue: config.QueueName,//队列
                        exchange: config.ExChangeName,//交换器
                        routingKey: config.RoutingKey,//routingkey
                        arguments: config.Arguments);
                }

                //持久化队列
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                //转换消息数据
                if (string.IsNullOrWhiteSpace(jsonBody()))
                    throw new Exception($"委托参数{nameof(jsonBody)}不可为空。");

                var body = Encoding.UTF8.GetBytes(jsonBody());
                //发送消息
                _channel.BasicPublish(
                    exchange: config.ExChangeName,
                    routingKey: config.RoutingKey,
                    mandatory: true,//如果exchange根据自身类型和消息routingKey无法找到一个合适的queue存储消息，那么broker会调用basic.return方法将消息返还给生产者;false:将直接丢弃
                    basicProperties: properties,
                    body: body);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, $"{nameof(MqService)}.Publish 出现异常");
                return false;
            }
        }
    }
}
