using Core.Interfaces;
using Core.NuggetAggregate;
using Core.NuggetAggregate.Exceptions;
using Core.NuggetAggregate.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime.Testing;

namespace Core.Tests.Domains.Nugget;

public class CreateAsyncShould
{
    private readonly FakeClock _clock;
    private readonly NuggetDomain _nuggetDomain;
    private readonly Mock<INuggetRepository> _nuggetRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IFileStorage> _fileStorage;

    private static readonly Guid GoodGuidUser = Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675");

    public CreateAsyncShould()
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
    }

    [Fact]
    public async Task CreateNuggetWithoutImage()
    {
        var newNuggetId = await _nuggetDomain.CreateAsync(
            new CreateNuggetCommand(
                "Title",
                "Content",
                GoodGuidUser,
                "FileNameImage",
                new MemoryStream()),
            CancellationToken.None);

        _fileStorage.Verify(f =>
                f.UploadFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MemoryStream>(),
                    CancellationToken.None),
            Times.Never);
        _nuggetRepository.Verify(e => e.CreateAsync(
                It.Is<NuggetAggregate.Nugget>(n => n.Id == newNuggetId &&
                                   n.UserId == GoodGuidUser &&
                                   n.Title == "Title" &&
                                   n.Content == "Content" &&
                                   n.UrlImage == null &&
                                   n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                                   n.UpdatedAt == null),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task CreateNuggetWithImage()
    {
        byte[] mockData = { 116, 101, 115, 116 };
        var newNuggetId = await _nuggetDomain.CreateAsync(
            new CreateNuggetCommand(
                "Title",
                "Content",
                GoodGuidUser,
                "FileNameImage",
                new MemoryStream(mockData)),
            CancellationToken.None);

        _fileStorage.Verify(f =>
                f.UploadFileAsync(
                    "nuggets-images",
                    "2020-03-06T14:13:00Z-FileNameImage",
                    It.IsAny<MemoryStream>(),
                    CancellationToken.None),
            Times.Once);

        _nuggetRepository.Verify(e => e.CreateAsync(
                It.Is<NuggetAggregate.Nugget>(n => n.Id == newNuggetId &&
                                   n.UserId == GoodGuidUser &&
                                   n.Title == "Title" &&
                                   n.Content == "Content" &&
                                   n.UrlImage == "https://nuggets-images.host/2020-03-06T14:13:00Z-FileNameImage" &&
                                   n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                                   n.UpdatedAt == null),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task NotBeCreatedException()
    {
        _nuggetRepository.Setup(r =>
                r.CreateAsync(
                    It.IsAny<NuggetAggregate.Nugget>(),
                    It.IsAny<CancellationToken>()))
            .Throws<Exception>();

        byte[] mockData = { 116, 101, 115, 116 };
        Func<Task> act = async () => await _nuggetDomain.CreateAsync(
            new CreateNuggetCommand(
                "Title",
                "Content",
                GoodGuidUser,
                "FileNameImage",
                new MemoryStream(mockData)),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotBeCreatedException>().WithMessage("The nugget could not be created.");

        _fileStorage.Verify(f =>
                f.UploadFileAsync(
                    "nuggets-images",
                    "2020-03-06T14:13:00Z-FileNameImage",
                    It.IsAny<MemoryStream>(),
                    CancellationToken.None),
            Times.Once);
        _fileStorage.Verify(f =>
                f.DeleteFileAsync(
                    "nuggets-images",
                    "2020-03-06T14:13:00Z-FileNameImage",
                    CancellationToken.None),
            Times.Once);

        _nuggetRepository.Verify(e => e.CreateAsync(
                It.Is<NuggetAggregate.Nugget>(n =>
                    n.UserId == GoodGuidUser &&
                    n.Title == "Title" &&
                    n.Content == "Content" &&
                    n.UrlImage == "https://nuggets-images.host/2020-03-06T14:13:00Z-FileNameImage" &&
                    n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                    n.UpdatedAt == null),
                CancellationToken.None),
            Times.Once);
    }
}