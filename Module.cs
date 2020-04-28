using Infrastructure.Utils.Core.Finder;
using Infrastructure.Utils.Core.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Infrastructure.Mq.RabbitMQ
{
    public class Module : ModuleBase
    {
        public override ModuleLevel ModuleLevel => ModuleLevel.Application;

        public override IServiceCollection AddModule(IServiceCollection services)
        {
            services.TryAddSingleton<IMqService, MqService>();
            return services;
        }

        public override void UseModule(IServiceProvider provider)
        {
            base.UseModule(provider);
            var assemblyFinder = provider.GetService<IAppAssemblyFinder>();

            //获取服务
            var mqService = provider.GetService<IMqService>();
            mqService.InitFactory();

            //获取实现了IConsumerDependency的对象，获取其 标记ConsumerAttribute的方法
            var consumers = assemblyFinder.FindTypes<IConsumerDependency>(t => typeof(IConsumerDependency).IsAssignableFrom(t) && t != typeof(IConsumerDependency));

            var interfaces = consumers.Where(x => x.IsInterface);
            Console.WriteLine($"获取到消费者服务接口{interfaces?.Count()}个");
            foreach (var @interface in interfaces)
            {
                try
                {
                    using (var scope = provider.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetService(@interface.GetTypeInfo());

                        var _ = consumers.Where(x => x.IsClass && @interface.GetTypeInfo().IsAssignableFrom(x.GetTypeInfo()))?.FirstOrDefault();
                        var members = _.GetMembers().Where(x => x.GetCustomAttribute(typeof(ConsumerAttribute)) != null);
                        Console.WriteLine($"从{@interface.GetTypeInfo().FullName}查找到{_.GetTypeInfo().FullName},并且获取到Consumer服务{members?.Count()}个");
                        foreach (var member in members)
                        {
                            var attr = member.GetCustomAttribute(typeof(ConsumerAttribute));
                            var attrQueueName = ((ConsumerAttribute)attr).QueueName;
                            var attrConsumerCount = ((ConsumerAttribute)attr).ConsumerCount;
                            var routingKey = ((ConsumerAttribute)attr).RoutingKey;
                            var autoAck = ((ConsumerAttribute)attr).AutoAck;

                            var methodName = member.Name;
                            MethodInfo m = _.GetMethod(methodName);
                            //先不考虑 direct的模式下指定routingKey的情况
                            mqService.Subscribe(() => new MqSettings(queueName: attrQueueName, autoAck: autoAck), jsonMsg =>
                                {
                                    try
                                    {
                                        var blnResult = (bool)m.Invoke(service, new object[] { jsonMsg });
                                        return blnResult;
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"{this.GetType().GetTypeInfo().FullName}.UseModel.consumerServer.Subscribe执行失败");
                                        return false;
                                    }
                                });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"扫描Consumer消费者过程中出现异常：{ex.Message}");
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}
