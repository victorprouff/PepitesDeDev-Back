using Core.Exceptions;
using Core.Interfaces;
using Core.NuggetAggregate;
using Core.NuggetAggregate.Exceptions;
using Core.NuggetAggregate.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime.Testing;

namespace Core.Tests.Domains.Nugget;

public class DeleteImageAsyncShould
{
    private readonly FakeClock _clock;
    private readonly NuggetDomain _nuggetDomain;
    private readonly Mock<INuggetRepository> _nuggetRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IFileStorage> _fileStorage;

    private static readonly Guid GoodGuidUser = Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675");
    private static readonly Guid BadGuidUser = Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC699");
    private static readonly Guid GoodGuidNugget = Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6");
    private static readonly Guid SecondGoodGuidNugget = Guid.Parse("93232D37-2468-41A3-8D4A-AECF85DDF0F8");
    private static readonly Guid BadGuidNugget = Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F");

    public DeleteImageAsyncShould()
    {
        _clock = FakeClock.FromUtc(2023, 3, 6, 14, 13, 0);

        _nuggetRepository = new Mock<INuggetRepository>();
        _userRepository = new Mock<IUserRepository>();
        _fileStorage = new Mock<IFileStorage>();

        _nuggetDomain = new NuggetDomain(
            _clock,
            new Mock<ILogger<NuggetDomain>>().Object,
            _nuggetRepository.Object,
            _userRepository.Object,
            _fileStorage.Object,
            "host");

        SetupNuggetRepositoryMethods();
    }

    [Fact]
    public async Task ThrowNotFoundException()
    {
        var act = async () => await _nuggetDomain.DeleteImageAsync(
            new DeleteNuggetImageCommand(BadGuidNugget, GoodGuidUser), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"The nugget with id {BadGuidNugget} is not found.");

        _nuggetRepository.Verify(
            e => e.GetById(BadGuidNugget, It.IsAny<CancellationToken>()),
            Times.Once);
        _userRepository.Verify(e => e.CheckIfIsAdmin(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        _fileStorage.Verify(f =>
                f.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _nuggetRepository.Verify(e => e.UpdateUrlImageAsync(
                It.IsAny<NuggetAggregate.Nugget>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ThrowNuggetDoesNotBelongToUserException()
    {
        var act = async () => await _nuggetDomain.DeleteImageAsync(
            new DeleteNuggetImageCommand(GoodGuidNugget, BadGuidUser), CancellationToken.None);

        await act.Should().ThrowAsync<NuggetDoesNotBelongToUserException>()
            .WithMessage($"The nugget with id {GoodGuidNugget} doesn't belong to the user with id {BadGuidUser}.");

        _nuggetRepository.Verify(
            e => e.GetById(GoodGuidNugget, It.IsAny<CancellationToken>()),
            Times.Once);
        _userRepository.Verify(e => e.CheckIfIsAdmin(
                BadGuidUser,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _fileStorage.Verify(f =>
                f.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _nuggetRepository.Verify(e => e.UpdateUrlImageAsync(
                It.IsAny<NuggetAggregate.Nugget>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DoesNothingBecauseUrlImageIsEmpty()
    {
        await _nuggetDomain.DeleteImageAsync(
            new DeleteNuggetImageCommand(GoodGuidNugget, GoodGuidUser), CancellationToken.None);

        _nuggetRepository.Verify(
            e => e.GetById(GoodGuidNugget, It.IsAny<CancellationToken>()),
            Times.Once);
        _userRepository.Verify(e => e.CheckIfIsAdmin(
                GoodGuidUser,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _fileStorage.Verify(f =>
                f.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _nuggetRepository.Verify(e => e.UpdateUrlImageAsync(
                It.IsAny<NuggetAggregate.Nugget>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteImage()
    {
        await _nuggetDomain.DeleteImageAsync(
            new DeleteNuggetImageCommand(SecondGoodGuidNugget, GoodGuidUser), CancellationToken.None);

        _nuggetRepository.Verify(
            e => e.GetById(SecondGoodGuidNugget, It.IsAny<CancellationToken>()),
            Times.Once);
        _userRepository.Verify(e => e.CheckIfIsAdmin(
                GoodGuidUser,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _fileStorage.Verify(f =>
                f.DeleteFileAsync("nuggets-images", "2020-03-06T14:13:00Z-FileNameImage",
                    It.IsAny<CancellationToken>()),
            Times.Once);
        _nuggetRepository.Verify(e => e.UpdateUrlImageAsync(
                It.Is<NuggetAggregate.Nugget>(n =>
                    n.Id == SecondGoodGuidNugget &&
                    n.Title == "Title" &&
                    n.Content == "Content" &&
                    n.UrlImage == null &&
                    n.UserId == GoodGuidUser &&
                    n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                    n.UpdatedAt == _clock.GetCurrentInstant()
                ),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private void SetupNuggetRepositoryMethods()
    {
        _nuggetRepository.Setup(r => r.GetById(
                GoodGuidNugget,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NuggetAggregate.Nugget(
                GoodGuidNugget,
                "Title",
                "Content",
                null,
                GoodGuidUser,
                FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                null));

        _nuggetRepository.Setup(r => r.GetById(
                SecondGoodGuidNugget,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NuggetAggregate.Nugget(
                SecondGoodGuidNugget,
                "Title",
                "Content",
                "https://nuggets-images.host/2020-03-06T14:13:00Z-FileNameImage",
                GoodGuidUser,
                FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                null));

        _nuggetRepository.Setup(r =>
                r.GetById(
                    BadGuidNugget,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((NuggetAggregate.Nugget)null!);

        _userRepository.Setup(r =>
                r.CheckIfIsAdmin(BadGuidUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepository.Setup(r =>
                r.CheckIfIsAdmin(GoodGuidUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }
}