using CloudApp.ServiceBus.Azure;
using System.Collections.Generic;
using System.Linq;
using CloudApp.ServiceBus.Contracts;
using CloudApp.ServiceBus.Infrastructure;

namespace CloudApp.ServiceBus
{
    public static class MessageQueueFactory
    {
        private static Dictionary<string, IMessageQueue> _Queues = new Dictionary<string, IMessageQueue>();

        public static IMessageQueue CreateInbound(string name, MessagePattern pattern, bool isTemporary = false,
                                                  Dictionary<string, object> properties = null)
        {
            var key = string.Format("{0}:{1}:{2}", Direction.Inbound, name, pattern);
            if (_Queues.ContainsKey(key))
                return _Queues[key];

            var queue = Create();
            queue.InitialiseInbound(name, pattern, isTemporary, properties);
            _Queues[key] = queue;
            return _Queues[key];
        }

        public static IMessageQueue CreateOutbound(string name, MessagePattern pattern, bool isTemporary = false,
                                                   Dictionary<string, object> properties = null)
        {
            var key = string.Format("{0}:{1}:{2}", Direction.Outbound, name, pattern);
            if (_Queues.ContainsKey(key))
                return _Queues[key];

            var queue = Create();
            queue.InitialiseOutbound(name, pattern, isTemporary,properties);
            _Queues[key] = queue;
            return _Queues[key];
        }

        public static void Delete(IMessageQueue queue)
        {
            queue.DeleteQueue();
            var clients = _Queues.Where(x => x.Value.Address == queue.Address).ToList();
            clients.ForEach(x =>
                {
                    x.Value.Dispose();
                    _Queues.Remove(x.Key);
                });
        }

        private static IMessageQueue Create()
        {
            //return new MsmqMessageQueue();
            //return new ZeroMqMessageQueue();
            //return new ServiceBusMessageQueue();
            //return new AwsMessageQueue();
            return new ServiceBusMessageQueue();
        }
    }
}