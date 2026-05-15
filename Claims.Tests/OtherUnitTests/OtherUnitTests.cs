using Claims.Repositories;
using Claims.Services;
using Claims.Validation;

namespace Claims.Tests.OtherUnitTests
{
    internal sealed class InMemoryClaimsRepository : IClaimsRepository
    {
        private readonly Dictionary<string, Claim> _store = new();

        public Task<IEnumerable<Claim>> GetAllAsync()
            => Task.FromResult<IEnumerable<Claim>>(_store.Values.ToList());

        public Task<Claim?> GetByIdAsync(string id)
            => Task.FromResult(_store.TryGetValue(id, out var c) ? c : null);

        public Task AddAsync(Claim claim)
        {
            _store[claim.Id] = claim;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            _store.Remove(id);
            return Task.CompletedTask;
        }
    }

    internal sealed class InMemoryCoversRepository : ICoversRepository
    {
        private readonly Dictionary<string, Cover> _store = new();

        public Task<IEnumerable<Cover>> GetAllAsync()
            => Task.FromResult<IEnumerable<Cover>>(_store.Values.ToList());

        public Task<Cover?> GetByIdAsync(string id)
            => Task.FromResult(_store.TryGetValue(id, out var c) ? c : null);

        public Task AddAsync(Cover cover)
        {
            _store[cover.Id] = cover;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            _store.Remove(id);
            return Task.CompletedTask;
        }

        public void Seed(Cover cover) => _store[cover.Id] = cover;
    }

    internal sealed class RecordingAuditService : IAuditService
    {
        public List<(string Id, string Method, string Entity)> Entries { get; } = new();

        public ValueTask AuditClaimAsync(string id, string httpRequestType, CancellationToken cancellationToken = default)
        {
            Entries.Add((id, httpRequestType, "Claim"));
            return ValueTask.CompletedTask;
        }

        public ValueTask AuditCoverAsync(string id, string httpRequestType, CancellationToken cancellationToken = default)
        {
            Entries.Add((id, httpRequestType, "Cover"));
            return ValueTask.CompletedTask;
        }
    }

    internal sealed class StubPremiumCalculator : IPremiumCalculator
    {
        public decimal Result { get; set; } = 42m;

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType) => Result;
    }
}
