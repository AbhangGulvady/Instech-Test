using System.Threading.Channels;

namespace Claims.Messaging
{
    
    public sealed class InMemoryAuditMessageBus : IAuditMessageBus
    {
        private readonly Channel<AuditMessage> _channel;

        public InMemoryAuditMessageBus()
        {
            _channel = Channel.CreateUnbounded<AuditMessage>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        }


        public ValueTask PublishAsync(AuditMessage message, CancellationToken cancellationToken = default)
        {
            var valueTask = _channel.Writer.WriteAsync(message, cancellationToken);
            return valueTask;
        }


        public IAsyncEnumerable<AuditMessage> ReadAllAsync(CancellationToken cancellationToken)
        { 
            var valueTask = _channel.Reader.ReadAllAsync(cancellationToken); 
            return valueTask;
        }
    }
}
