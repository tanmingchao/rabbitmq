// -----------------------------------------------------------------------
//  <copyright file="MqSettings.cs" company="禾卓软件科技有限公司">
//      Copyright (c) 2020 禾卓软件科技有限公司. All rights reserved.
//  </copyright>
//  <last-editor>TANMINGCHAO - 2020/4/28 16:29:38 </last-editor>
// -----------------------------------------------------------------------

namespace Infrastructure.Mq.RabbitMQ
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="MqSettings" />.
    /// </summary>
    public class MqSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MqSettings"/> class.
        /// </summary>
        /// <param name="queueName">The queueName<see cref="string"/>.</param>
        public MqSettings(string queueName)
        {
            QueueName = queueName;
            ExChangeName = queueName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MqSettings"/> class.
        /// </summary>
        /// <param name="exChangeName">The exChangeName<see cref="string"/>.</param>
        /// <param name="queueName">The queueName<see cref="string"/>.</param>
        public MqSettings(string exChangeName, string queueName)
        {
            ExChangeName = exChangeName;
            QueueName = queueName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MqSettings"/> class.
        /// </summary>
        /// <param name="queueName">The queueName<see cref="string"/>.</param>
        /// <param name="autoAck">The autoAck<see cref="bool"/>.</param>
        public MqSettings(string queueName, bool autoAck)
        {
            ExChangeName = queueName;
            QueueName = queueName;
            AutoAck = autoAck;
        }

        /// <summary>
        /// Gets or sets the ExChangeName
        /// 交换属性 - 交换器名称.
        /// </summary>
        public string ExChangeName { get; set; }

        /// <summary>
        /// Gets or sets the ExChangeType
        /// 交换属性 - 交互类型.
        /// </summary>
        public string ExChangeType { get; set; } = "fanout";

        /// <summary>
        /// Gets or sets the QueueName
        /// 声明队列属性 - 队列名称.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Durable
        /// 声明队列属性 - 是否持久化 true.
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Exclusive
        /// 声明队列属性 - 是否排它性 false.
        /// </summary>
        public bool Exclusive { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether AutoDelete
        /// 声明队列属性 - 是否自动删除 false.
        /// </summary>
        public bool AutoDelete { get; set; } = false;

        /// <summary>
        /// Gets or sets the RoutingKey
        /// 绑定消息队列属性 - 路由key.
        /// </summary>
        public string RoutingKey { get; set; } = "*";

        /// <summary>
        /// Gets or sets the Arguments.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; }

        //------------以下消费端独有-----------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether AutoAck.
        /// </summary>
        public bool AutoAck { get; set; } = true;
    }
}
