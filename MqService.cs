// -----------------------------------------------------------------------
//  <copyright file="MqService.cs" company="禾卓软件科技有限公司">
//      Copyright (c) 2020 禾卓软件科技有限公司. All rights reserved.
//  </copyright>
//  <last-editor>TANMINGCHAO - 2020/4/28 16:29:38 </last-editor>
// -----------------------------------------------------------------------

namespace Infrastructure.Mq.RabbitMQ
{
    using Infrastructure.Utils.Core;
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    /// Defines the <see cref="MqService" />.
    /// </summary>
    public partial class MqService : IMqService
    {
        /// <summary>
        /// Defines the _factory.
        /// </summary>
        internal ConnectionFactory _factory;

        /// <summary>
        /// Defines the _connection.
        /// </summary>
        internal IConnection _connection;

        /// <summary>
        /// Defines the _channel.
        /// </summary>
        internal IModel _channel;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        internal ILogger<MqService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqService"/> class.
        /// </summary>
        /// <param name="logger">The logger<see cref="ILogger{MqService}"/>.</param>
        public MqService(ILogger<MqService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// The InitFactory.
        /// </summary>
        public void InitFactory()
        {
            if (_factory == null)
            {
                var host = AppSettingsManager.Get("MQSetting:Host");
                var port = AppSettingsManager.Get("MQSetting:Port");
                var userName = AppSettingsManager.Get("MQSetting:UserName");
                var password = AppSettingsManager.Get("MQSetting:Password");
                var requestedHeartbeat = AppSettingsManager.Get("MQSetting:RequestedHeartbeat");
                var mqUri = AppSettingsManager.Get("MQSetting:MQUri");
                var sslUril = AppSettingsManager.Get("MQSetting:SSLUri");

                _factory = new ConnectionFactory()
                {
                    HostName = host,
                    Port = Convert.ToInt32(port),
                    UserName = userName,
                    Password = password,
                    //Endpoint = new AmqpTcpEndpoint(new Uri(host)),
                };

                if (!string.IsNullOrWhiteSpace(requestedHeartbeat))
                    _factory.RequestedHeartbeat = (ushort)Convert.ToInt32(requestedHeartbeat);
                if (!string.IsNullOrWhiteSpace(mqUri))
                    _factory.Uri = new Uri(mqUri);

                // connection
                if (_connection == null || !_connection.IsOpen)
                {
                    _connection = _factory.CreateConnection();
                    _connection.ConnectionShutdown += (sender, args) =>
                    {
                        try
                        {
                            //清除连接及频道
                            CleanupResource();
                            _logger.LogWarning($"mq连接已停止，占用资源已释放");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(exception: ex, "尝试连接RabbitMQ服务器出现错误");
                        }
                    };
                }
            }
        }

        /// <summary>
        /// 清理资源.
        /// </summary>
        private void CleanupResource()
        {
            if (_connection != null && _connection.IsOpen)
            {
                try
                {
                    _connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(exception: ex, "尝试关闭RabbitMQ连接遇到错误");
                }
                _connection = null;
            }
        }
    }
}
