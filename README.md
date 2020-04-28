使用方式：任意Service的IService接口对象实现 IConsumerDependency，然后，添加对象 infrastructure.Mq.RabbitMQ的包的引用，同时在Service的对应方法上加上[Consumer("对列名称")]即可。
比如：
 public interface ISysDictionaryService : IServiceDependency,IConsumerDependency
    {
        bool TestRequest();        
        bool Test(string msg);
     }
 public class SysDictionaryService : ISysDictionaryService
 {
        [Consumer("SysDictionaryService.queue.TestRequest")]
        public bool TestRequest(string msg){return true;}
        
        [Consumer("SysDictionaryService.queue.Test")]
        public bool Test(string msg){return true;}
 }
