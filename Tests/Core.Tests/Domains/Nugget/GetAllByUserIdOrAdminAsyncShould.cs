using Core.Interfaces;
using Core.NuggetAggregate;
using Core.NuggetAggregate.Projections;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime.Testing;

namespace Core.Tests.Domains.Nugget;

public class GetAllByUserIdOrAdminAsyncShould
{
    private readonly NuggetDomain _nuggetDomain;
    private readonly Mock<INuggetRepository> _nuggetRepository;

    private static readonly Guid FirstGuidUser = Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675");
    private static readonly Guid SecondGuidUser = Guid.Parse("E3083B9B-50B2-4AB5-A1D3-572203496580");
    private static readonly Guid FirstGuidNugget = Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6");
    private static readonly Guid SecondGuidNugget = Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F");
    private static readonly Guid ThirdGuidNugget = Guid.Parse("5C6D8C01-BDD5-40AE-A9DC-B3C16C54EA02");

    public GetAllByUserIdOrAdminAsyncShould()
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
        var act = await _nuggetDomain.GetAllByUserIdOrAdminAsync(FirstGuidUser, 2, 0, CancellationToken.None);

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
                    ThirdGuidNugget,
                    "Title3",
                    "Content3",
                    true,
                    "Url2",
                    "The Creator 3",
                    FirstGuidUser,
                    FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                    null)
            });

        act.Should().BeEquivalentTo(expectedProjection);

        _nuggetRepository.Verify(e => e.GetAllByUserIdProjection(FirstGuidUser, 2, 0, CancellationToken.None), Times.Once);
    }

    private void SetupNuggetRepositoryMethods()
    {
        var nuggets = new List<NuggetAggregate.Projections.Nugget>()
        {
            new(
                FirstGuidNugget,
                "Title",
                "Content",
                true,
                null,
                "The Creator",
                FirstGuidUser,
                FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                null),
            new(
                SecondGuidNugget,
                "Title2",
                "Content2",
                true,
                "Url",
                "The Creator 2",
                SecondGuidUser,
                FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                null),
            new(
                ThirdGuidNugget,
                "Title3",
                "Content3",
                true,
                "Url2",
                "The Creator 3",
                FirstGuidUser,
                FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                null)
        };

        _nuggetRepository.Setup(e => e.GetAllByUserIdProjection(FirstGuidUser, 2, 0, CancellationToken.None))
            .ReturnsAsync(new GetAllNuggetsProjection(2, nuggets.Where(n => n.UserId == FirstGuidUser)));
    }
}