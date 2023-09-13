using Core.Interfaces;
using Core.NuggetAggregate;
using Core.NuggetAggregate.Projections;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime.Testing;

namespace Core.Tests.Domains.Nugget;

public class GetAllAsyncShould
{
    private readonly NuggetDomain _nuggetDomain;
    private readonly Mock<INuggetRepository> _nuggetRepository;

    private static readonly Guid FirstGuidUser = Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675");
    private static readonly Guid SecondGuidUser = Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675");
    private static readonly Guid FirstGuidNugget = Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6");
    private static readonly Guid SecondGuidNugget = Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F");

    public GetAllAsyncShould()
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
    public async Task ReturnGetAllNuggetsProjection()
    {
        var act = await _nuggetDomain.GetAllAsync(true, 2, 0, CancellationToken.None);

        var expectedProjection = new GetAllNuggetsProjection(
            2,
            new[]
            {
                new NuggetAggregate.Projections.Nugget(
                    FirstGuidNugget,
                    "Title",
                    "Content",
                    true,
                    null,
                    "The Creator",
                    FirstGuidUser,
                    FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                    null),
                new NuggetAggregate.Projections.Nugget(
                    SecondGuidNugget,
                    "Title2",
                    "Content2",
                    false,
                    "Url",
                    "The Creator 2",
                    SecondGuidUser,
                    FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                    null)
            });

        act.Should().BeEquivalentTo(expectedProjection);

        _nuggetRepository.Verify(e => e.GetAll(true, 2, 0, CancellationToken.None), Times.Once);
    }
    
    private void SetupNuggetRepositoryMethods()
    {
        _nuggetRepository.Setup(e => e.GetAll(true, 2, 0, CancellationToken.None))
            .ReturnsAsync(
                new GetAllNuggetsProjection(
                    2,
                    new[]
                    {
                        new NuggetAggregate.Projections.Nugget(
                            FirstGuidNugget,
                            "Title",
                            "Content",
                            true,
                            null,
                            "The Creator",
                            FirstGuidUser,
                            FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                            null),
                        new NuggetAggregate.Projections.Nugget(
                            SecondGuidNugget,
                            "Title2",
                            "Content2",
                            false,
                            "Url",
                            "The Creator 2",
                            SecondGuidUser,
                            FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                            null)
                    }));
    }
}