using Claims.Messaging;
using Xunit;

namespace Claims.Tests
{
    public class InMemoryAuditMessageBusTests
    {
        [Fact]
        public async Task PublishedMessage_IsReadable()
        {
            var bus = new InMemoryAuditMessageBus();
            var message = new AuditMessage(AuditEntityType.Claim, "id-1", "POST", DateTime.UtcNow);

            await bus.PublishAsync(message);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            await foreach (var read in bus.ReadAllAsync(cts.Token))
            {
                Assert.Equal(message, read);
                return;
            }

            Assert.Fail("No message was read from the bus.");
        }
    }
}
