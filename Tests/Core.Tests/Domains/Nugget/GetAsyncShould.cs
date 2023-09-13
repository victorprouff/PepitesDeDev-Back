using Core.Exceptions;
using Core.Interfaces;
using Core.NuggetAggregate;
using Core.NuggetAggregate.Projections;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime.Testing;

namespace Core.Tests.Domains.Nugget;

public class GetAsyncShould
{
    private readonly NuggetDomain _nuggetDomain;
    private readonly Mock<INuggetRepository> _nuggetRepository;

    private static readonly Guid GoodGuidUser = Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675");
    private static readonly Guid GoodGuidNugget = Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6");
    private static readonly Guid BadGuidNugget = Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F");

    public GetAsyncShould()
    {
        var clock = FakeClock.FromUtc(2020, 3, 6, 14, 13, 0);

        _nuggetRepository = new Mock<INuggetRepository>();

        _nuggetDomain = new NuggetDomain(
            clock,
            new Mock<ILogger<NuggetDomain>>().Object,
            _nuggetRepository.Object,
            new Mock<IUserRepository>().Object,
            new Mock<IFileStorage>().Object,
            "host");

        SetupNuggetRepositoryMethods();
    }

    [Fact]
    public async Task ThrowNotFoundException()
    {
        var act = async () => await _nuggetDomain.GetAsync(BadGuidNugget, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"The nugget with id {BadGuidNugget} is not found.");
        
        _nuggetRepository.Verify(e => e.GetByIdProjection(BadGuidNugget, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ReturnNugget()
    {
        var act = await _nuggetDomain.GetAsync(GoodGuidNugget, CancellationToken.None);

        var expectedProjection = new GetNuggetProjection(
            GoodGuidNugget,
            GoodGuidUser,
            "Title",
            "Content",
            true,
            null,
            "The Creator",
            FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
            null);

        act.Should().BeEquivalentTo(expectedProjection);
        
        _nuggetRepository.Verify(e => e.GetByIdProjection(GoodGuidNugget, CancellationToken.None), Times.Once);
    }
    
    private void SetupNuggetRepositoryMethods()
    {
        _nuggetRepository.Setup(e => e.GetByIdProjection(GoodGuidNugget, CancellationToken.None))
            .ReturnsAsync(
                new GetNuggetProjection(
                    GoodGuidNugget,
                    GoodGuidUser,
                    "Title",
                    "Content",
                    true,
                    null,
                    "The Creator",
                    FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                    null));
    }
}