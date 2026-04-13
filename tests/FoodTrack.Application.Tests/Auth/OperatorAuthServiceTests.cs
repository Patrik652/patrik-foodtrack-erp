using FluentAssertions;
using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Application.Auth;
using FoodTrack.Domain.Entities;

namespace FoodTrack.Application.Tests.Auth;

public sealed class OperatorAuthServiceTests
{
    [Fact]
    public async Task AuthenticateAsync_ShouldReturnOperatorForValidCredentials()
    {
        var operatorAccount = OperatorAccount.Create("op-1001", "Roman Skladnik", "1234");
        var service = new OperatorAuthService(new FakeOperatorAccountRepository(operatorAccount));

        var result = await service.AuthenticateAsync(
            new AuthenticateOperatorCommand(" OP-1001 ", "1234"),
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.BadgeCode.Should().Be("OP-1001");
        result.DisplayName.Should().Be("Roman Skladnik");
        result.OperatorId.Should().Be(operatorAccount.Id);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnNullForInvalidPin()
    {
        var operatorAccount = OperatorAccount.Create("OP-1001", "Roman Skladnik", "1234");
        var service = new OperatorAuthService(new FakeOperatorAccountRepository(operatorAccount));

        var result = await service.AuthenticateAsync(
            new AuthenticateOperatorCommand("OP-1001", "9999"),
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnNullForInactiveOperator()
    {
        var inactiveAccount = OperatorAccount.Create("OP-1999", "Archivovany Operator", "1234", isActive: false);
        var service = new OperatorAuthService(new FakeOperatorAccountRepository(inactiveAccount));

        var result = await service.AuthenticateAsync(
            new AuthenticateOperatorCommand("OP-1999", "1234"),
            CancellationToken.None);

        result.Should().BeNull();
    }

    private sealed class FakeOperatorAccountRepository(params OperatorAccount[] operatorAccounts) : IOperatorAccountRepository
    {
        public Task<OperatorAccount?> GetByBadgeCodeAsync(string badgeCode, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                operatorAccounts.SingleOrDefault(account => account.BadgeCode == badgeCode.Trim().ToUpperInvariant()));
        }
    }
}
