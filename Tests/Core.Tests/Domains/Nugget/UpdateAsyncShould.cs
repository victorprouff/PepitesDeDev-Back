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

public class UpdateAsyncShould
{
    private readonly FakeClock _clock;
    private readonly NuggetDomain _nuggetDomain;
    private readonly Mock<INuggetRepository> _nuggetRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IFileStorage> _fileStorage;

    private static readonly Guid GoodGuidUser = Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675");
    private static readonly Guid BadGuidUser = Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC699");
    private static readonly Guid GoodGuidNugget = Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6");
    private static readonly Guid BadGuidNugget = Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F");

    public UpdateAsyncShould()
    {
        _clock = FakeClock.FromUtc(2020, 3, 6, 14, 13, 0);

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
        var act = async () => await _nuggetDomain.UpdateAsync(
            new UpdateNuggetCommand(
                BadGuidNugget,
                GoodGuidUser,
                "Title",
                "Content",
                "FileNameImage",
                new MemoryStream()),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"The nugget with id {BadGuidNugget} is not found.");

        _fileStorage.Verify(f =>
                f.UploadFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MemoryStream>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);
        _fileStorage.Verify(f =>
                f.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _userRepository.Verify(e => e.CheckIfIsAdmin(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        _nuggetRepository.Verify(
            e => e.GetById(BadGuidNugget, It.IsAny<CancellationToken>()),
            Times.Once);
        _nuggetRepository.Verify(e => e.UpdateAsync(
                It.IsAny<NuggetAggregate.Nugget>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateNuggetDoesNotBelongToUserExceptionWithNotGoodUser()
    {
        var act = async () => await _nuggetDomain.UpdateAsync(
            new UpdateNuggetCommand(
                GoodGuidNugget,
                BadGuidUser,
                "newTitle",
                "newContent",
                "newFileNameImage",
                new MemoryStream()),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<NuggetDoesNotBelongToUserException>()
            .WithMessage(
                $"The nugget with id {GoodGuidNugget} doesn't belong to the user with id {BadGuidUser}.");

        _fileStorage.Verify(f =>
                f.UploadFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MemoryStream>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);
        _fileStorage.Verify(f =>
                f.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _userRepository.Verify(e => e.CheckIfIsAdmin(
                BadGuidUser,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _nuggetRepository.Verify(e => e.UpdateAsync(
                It.IsAny<NuggetAggregate.Nugget>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateNuggetWithNormalUser()
    {
        byte[] mockData = { 116, 101, 115, 116 };

        await _nuggetDomain.UpdateAsync(
            new UpdateNuggetCommand(
                GoodGuidNugget,
                GoodGuidUser,
                "newTitle",
                "newContent",
                "newFileNameImage",
                new MemoryStream(mockData)),
            CancellationToken.None);

        _nuggetRepository.Verify(
            e => e.GetById(GoodGuidNugget, It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepository.Verify(e => e.CheckIfIsAdmin(
                GoodGuidUser,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _fileStorage.Verify(f =>
                f.UploadFileAsync(
                    "nuggets-images",
                    "2020-03-06T14:13:00Z-newFileNameImage",
                    It.IsAny<MemoryStream>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        _fileStorage.Verify(f =>
                f.DeleteFileAsync(
                    "nuggets-images",
                    "2020-03-06T14:13:00Z-FileNameImage",
                    It.IsAny<CancellationToken>()),
            Times.Once);

        _nuggetRepository.Verify(e => e.UpdateAsync(
                It.Is<NuggetAggregate.Nugget>(n => n.UserId == GoodGuidUser &&
                                   n.Title == "newTitle" &&
                                   n.Content == "newContent" &&
                                   n.UrlImage == "https://nuggets-images.host/2020-03-06T14:13:00Z-newFileNameImage" &&
                                   n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                                   n.UpdatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant()),
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
                true,
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