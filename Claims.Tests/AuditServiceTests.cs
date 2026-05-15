using Claims.Messaging;
using Claims.Services;
using Xunit;

namespace Claims.Tests
{
    public class AuditServiceTests
    {
        private sealed class CapturingBus : IAuditMessageBus
        {
            public List<AuditMessage> Published { get; } = new();

            public ValueTask PublishAsync(AuditMessage message, CancellationToken cancellationToken = default)
            {
                Published.Add(message);
                return ValueTask.CompletedTask;
            }

            public IAsyncEnumerable<AuditMessage> ReadAllAsync(CancellationToken cancellationToken)
                => throw new NotSupportedException();
        }

        [Fact]
        public async Task AuditClaimAsync_PublishesClaimMessage()
        {
            var bus = new CapturingBus();
            var auservice = new AuditService(bus);

            await auservice.AuditClaimAsync("claim-1", "POST");

            var msg = Assert.Single(bus.Published);
            Assert.Equal(AuditEntityType.Claim, msg.EntityType);
            Assert.Equal("claim-1", msg.EntityId);
            Assert.Equal("POST", msg.HttpRequestType);
        }

        [Fact]
        public async Task AuditCoverAsync_PublishesCoverMessage()
        {
            var bus = new CapturingBus();
            var sut = new AuditService(bus);

            await sut.AuditCoverAsync("cover-1", "DELETE");

            var msg = Assert.Single(bus.Published);
            Assert.Equal(AuditEntityType.Cover, msg.EntityType);
            Assert.Equal("cover-1", msg.EntityId);
            Assert.Equal("DELETE", msg.HttpRequestType);
        }
    }
}
