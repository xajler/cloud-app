using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CloudApp.ServiceBus.Contracts;
using CloudApp.ServiceBus.Infrastructure;
using CloudApp.ServiceBus.Infrastructure.Extensions;

namespace CloudApp.ServiceBus.Azure
{
    public class ServiceBusMessageQueue : MessageQueueBase
    {
        private QueueClient _queueClient;
        private TopicClient _topicClient;
        private SubscriptionClient _subscriptionClient;
        private const string _connectionString = "YOUR-CONNECTION-STRING";
        private Action<Message> _handleMessage;

        public override void InitialiseOutbound(string name, MessagePattern pattern, bool isTemporary, 
            Dictionary<string, object> properties = null)
        {
            Initialise(Direction.Outbound, name, pattern, isTemporary, properties);
            var factory = MessagingFactory.CreateFromConnectionString(_connectionString);
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _topicClient = factory.CreateTopicClient(Address);
            }
            else
            {
                _queueClient = factory.CreateQueueClient(Address);
            }
        }

        public override void InitialiseInbound(string name, MessagePattern pattern, bool isTemporary, 
            Dictionary<string, object> properties = null)
        {
            Initialise(Direction.Inbound, name, pattern, isTemporary, properties);
            var factory = MessagingFactory.CreateFromConnectionString(_connectionString);
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                var addressParts = Address.Split(':');
                _subscriptionClient = factory.CreateSubscriptionClient(addressParts[0], addressParts[1]);
            }
            else
            {
                _queueClient = factory.CreateQueueClient(Address);
            }
        }

        public override void Send(Message message)
        {
            var brokeredMessage = new BrokeredMessage(message.ToJsonStream(), true);
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _topicClient.Send(brokeredMessage);
            }
            else
            {
                _queueClient.Send(brokeredMessage);
            }
        }

        protected override void ListenInternal(Action<Message> onMessageReceived, 
            CancellationToken cancellationToken)
        {
            _handleMessage = onMessageReceived;
            var options = new OnMessageOptions
            {
                MaxConcurrentCalls = 10
            };
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                _subscriptionClient.OnMessage(Handle, options);
            }
            else
            {
                _queueClient.OnMessage(Handle, options);
            }
            cancellationToken.WaitHandle.WaitOne();
        }

        public override void Receive(Action<Message> onMessageReceived, bool processAsync, 
            int maximumWaitMilliseconds = 0)
        {
            _handleMessage = onMessageReceived;
            BrokeredMessage brokeredMessage;
            if (Pattern == MessagePattern.PublishSubscribe)
            {
                brokeredMessage = _subscriptionClient.Receive(
                    TimeSpan.FromMilliseconds(maximumWaitMilliseconds));
            }
            else
            {
                brokeredMessage = _queueClient.Receive(
                    TimeSpan.FromMilliseconds(maximumWaitMilliseconds));
            }
            if (processAsync)
            {
                Task.Factory.StartNew(() => Handle(brokeredMessage));
            }
            else
            {
                Handle(brokeredMessage);
            }
        }

        private void Handle(BrokeredMessage brokeredMessage)
        {
            var messageStream = brokeredMessage.GetBody<Stream>();
            var message = Message.FromJson(messageStream);
            _handleMessage(message);
            brokeredMessage.Complete();
        }

        public override string GetAddress(string name)
        {
            switch (name.ToLower())
            {
                case "doesuserexist":
                    return "doesuserexist";

                case "unsubscribe":
                    return "unsubscribe";

                case "unsubscribed-event":
                    return "unsubscribed-event";

                case "unsubscribe-crm":
                    return "unsubscribed-event:crm";

                case "unsubscribe-fulfilment":
                    return "unsubscribed-event:fulfilment";

                case "unsubscribe-legacy":
                    return "unsubscribed-event:legacy";

                default:
                    return name;
            }
        }

        public override IMessageQueue GetResponseQueue()
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Outbound))
                throw new InvalidOperationException("Cannot get a response queue except for outbound request-response");

            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            var responseAddress = Guid.NewGuid().ToString().Substring(0, 6);
            namespaceManager.CreateQueue(responseAddress);

            var responseQueue = MessageQueueFactory.CreateInbound(responseAddress, 
                MessagePattern.RequestResponse, true);
            return responseQueue;
        }

        public override IMessageQueue GetReplyQueue(Message message)
        {
            var replyQueue = MessageQueueFactory.CreateOutbound(message.ResponseAddress, 
                MessagePattern.RequestResponse, true);
            return replyQueue;
        }

        public override void DeleteQueue()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            if (namespaceManager.QueueExists(Address))
            {
                namespaceManager.DeleteQueue(Address);
            }
        }
        protected override void Dispose(bool disposing)
        {
            //
        }
    }
}
