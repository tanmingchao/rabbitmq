// -----------------------------------------------------------------------
//  <copyright file="SysUser.cs" company="享佳健康">
//      Copyright (c) 2020 享佳健康. All rights reserved.
//  </copyright>
//  <last-editor>TANMINGCHAO - 2020/3/25 12:07:48 </last-editor>
// -----------------------------------------------------------------------

namespace Infrastructure.Mq.RabbitMQ
{
    using System;

    /// <summary>
    /// Defines the <see cref="ConsumerAttribute" />
    /// </summary>
    [AttributeUsage(/*AttributeTargets.Class |*/ AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ConsumerAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the QueueName
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the ConsumerCount
        /// 消费者初始化个数 默认1个
        /// </summary>
        public int ConsumerCount { get; set; } = 1;

        /// <summary>
        /// Gets or sets the RoutingKey
        /// routingKey
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerAttribute"/> class.
        /// </summary>
        /// <param name="queueName">The queueName<see cref="string"/></param>
        public ConsumerAttribute(string queueName) : this(queueName, 1)
        {
            QueueName = queueName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerAttribute"/> class.
        /// </summary>
        /// <param name="queueName">The queueName<see cref="string"/></param>
        /// <param name="consumerCount">The consumerCount<see cref="int"/></param>
        public ConsumerAttribute(string queueName, int consumerCount)
        {
            QueueName = queueName;
            ConsumerCount = consumerCount;
        }

        /// <summary>
        /// Gets the TypeId
        /// </summary>
        public override object TypeId => Guid.NewGuid();

        public bool AutoAck { get; set; } = true;
    }
}
