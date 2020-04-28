// -----------------------------------------------------------------------
//  <copyright file="IMqService.cs" company="禾卓软件科技有限公司">
//      Copyright (c) 2020 禾卓软件科技有限公司. All rights reserved.
//  </copyright>
//  <last-editor>谭明超 - 2020/4/28 16:28:53 </last-editor>
// -----------------------------------------------------------------------

namespace Infrastructure.Mq.RabbitMQ
{
    using System;

    /// <summary>
    /// Defines the <see cref="IMqService" />.
    /// </summary>
    public interface IMqService
    {
        /// <summary>
        /// The InitFactory.
        /// </summary>
        void InitFactory();

        /// <summary>
        /// The Subscribe.
        /// </summary>
        /// <param name="queueName">The queueName<see cref="string"/>.</param>
        /// <param name="func">The func<see cref="Func{string, bool}"/>.</param>
        void Subscribe(string queueName, Func<string, bool> func);

        /// <summary>
        /// The Subscribe.
        /// </summary>
        /// <param name="settings">The settings<see cref="Func{MqSettings}"/>.</param>
        /// <param name="func">The func<see cref="Func{string, bool}"/>.</param>
        void Subscribe(Func<MqSettings> settings, Func<string, bool> func);

        //void Subscribe<TObject>(Func<MqSettings> settings, Func<TObject, bool> func);
        /// <summary>
        /// The Publish.
        /// </summary>
        /// <param name="queueName">The queueName<see cref="string"/>.</param>
        /// <param name="jsonBody">The jsonBody<see cref="Func{string}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool Publish(string queueName, Func<string> jsonBody);

        /// <summary>
        /// The Publish.
        /// </summary>
        /// <param name="produceConfig">The produceConfig<see cref="Func{MqSettings}"/>.</param>
        /// <param name="json">The json<see cref="Func{string}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool Publish(Func<MqSettings> produceConfig, Func<string> json);

        /// <summary>
        /// The Publish.
        /// </summary>
        /// <typeparam name="TObject">.</typeparam>
        /// <param name="produceConfig">The produceConfig<see cref="Func{MqSettings}"/>.</param>
        /// <param name="jsonBody">The jsonBody<see cref="Func{string}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool Publish<TObject>(Func<MqSettings> produceConfig, Func<string> jsonBody);
    }
}
