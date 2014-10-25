using System;
using System.Collections.Generic;
using System.Threading;
using CloudApp.ServiceBus.Infrastructure;

namespace CloudApp.ServiceBus.Contracts
{
    public interface IMessageQueue : IDisposable
    {
        string Address { get; }

        Dictionary<string, object> Properties { get; }

        void InitialiseOutbound(string address, MessagePattern pattern, bool isTemporary, 
            Dictionary<string, object> properties = null);

        void InitialiseInbound(string address, MessagePattern pattern,  bool isTemporary, 
            Dictionary<string, object> properties = null);
        
        void Send(Message message);

        void Listen(Action<Message> onMessageReceived, CancellationToken cancellationToken);

        void Receive(Action<Message> onMessageReceived, int maximumWaitMilliseconds = 0);

        string GetAddress(string name);

        IMessageQueue GetResponseQueue();

        IMessageQueue GetReplyQueue(Message message);

        void DeleteQueue();
    }
}
