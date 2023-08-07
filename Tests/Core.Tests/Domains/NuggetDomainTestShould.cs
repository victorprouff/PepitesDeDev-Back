using Core.Exceptions;
using Core.Interfaces;
using Core.NuggetAggregate;
using Core.NuggetAggregate.Exceptions;
using Core.NuggetAggregate.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime.Testing;

namespace Core.Tests.Domains;

public class NuggetDomainTestShould
{
    private readonly FakeClock _clock;
    private readonly NuggetDomain _nuggetDomain;
    private readonly Mock<INuggetRepository> _nuggetRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IFileStorage> _fileStorage;

    public NuggetDomainTestShould()
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
    public async Task CreateAsyncCreateNuggetWithoutImage()
    {
        var newNuggetId = await _nuggetDomain.CreateAsync(
            new CreateNuggetCommand(
                "Title",
                "Content",
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
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
                It.Is<Nugget>(n => n.Id == newNuggetId &&
                                   n.UserId == Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675") &&
                                   n.Title == "Title" &&
                                   n.Content == "Content" &&
                                   n.UrlImage == null &&
                                   n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                                   n.UpdatedAt == null),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsyncCreateNuggetWithImage()
    {
        byte[] mockData = { 116, 101, 115, 116 };
        var newNuggetId = await _nuggetDomain.CreateAsync(
            new CreateNuggetCommand(
                "Title",
                "Content",
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
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
                It.Is<Nugget>(n => n.Id == newNuggetId &&
                                   n.UserId == Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675") &&
                                   n.Title == "Title" &&
                                   n.Content == "Content" &&
                                   n.UrlImage == "https://nuggets-images.host/2020-03-06T14:13:00Z-FileNameImage" &&
                                   n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                                   n.UpdatedAt == null),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsyncNotBeCreatedException()
    {
        _nuggetRepository.Setup(r =>
                r.CreateAsync(
                    It.IsAny<Nugget>(),
                    It.IsAny<CancellationToken>()))
            .Throws<Exception>();

        byte[] mockData = { 116, 101, 115, 116 };
        Func<Task> act = async () => await _nuggetDomain.CreateAsync(
            new CreateNuggetCommand(
                "Title",
                "Content",
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
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
                It.Is<Nugget>(n =>
                    n.UserId == Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675") &&
                    n.Title == "Title" &&
                    n.Content == "Content" &&
                    n.UrlImage == "https://nuggets-images.host/2020-03-06T14:13:00Z-FileNameImage" &&
                    n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                    n.UpdatedAt == null),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncThrowNotFoundException()
    {
        var act = async () => await _nuggetDomain.UpdateAsync(
            new UpdateNuggetCommand(
                Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F"),
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
                "Title",
                "Content",
                "FileNameImage",
                new MemoryStream()),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The nugget with id DE540E54-DCF8-4717-AF50-332BCC1DFE2F is not found.");

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
            e => e.GetById(Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F"), It.IsAny<CancellationToken>()),
            Times.Once);
        _nuggetRepository.Verify(e => e.UpdateAsync(
                It.IsAny<Nugget>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsyncNuggetDoesNotBelongToUserExceptionWithNotGoodUser()
    {
        var act = async () => await _nuggetDomain.UpdateAsync(
            new UpdateNuggetCommand(
                Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6"),
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC699"),
                "newTitle",
                "newContent",
                "newFileNameImage",
                new MemoryStream()),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<NuggetDoesNotBelongToUserException>()
            .WithMessage(
                $"The nugget with id 3F7132FC-7F73-40BE-88A8-F94D8CC933D6 doesn't belong to the user with id DAEAE728-BF12-4620-837A-81A6614CC699.");

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
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC699"),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _nuggetRepository.Verify(e => e.UpdateAsync(
                It.IsAny<Nugget>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsyncUpdateNuggetWithNormalUser()
    {
        byte[] mockData = { 116, 101, 115, 116 };

        await _nuggetDomain.UpdateAsync(
            new UpdateNuggetCommand(
                Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6"),
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
                "newTitle",
                "newContent",
                "newFileNameImage",
                new MemoryStream(mockData)),
            CancellationToken.None);

        _nuggetRepository.Verify(
            e => e.GetById(Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6"), It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepository.Verify(e => e.CheckIfIsAdmin(
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
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
                It.Is<Nugget>(n => n.UserId == Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675") &&
                                   n.Title == "newTitle" &&
                                   n.Content == "newContent" &&
                                   n.UrlImage == "https://nuggets-images.host/2020-03-06T14:13:00Z-newFileNameImage" &&
                                   n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                                   n.UpdatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant()),
            It.IsAny<CancellationToken>()),
        Times.Once);
    }

    [Fact]
    public async Task UpdateImageAsyncThrowNotFoundException()
    {
        var act = async () => await _nuggetDomain.UpdateImageAsync(
            new UpdateNuggetImageCommand(
                Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F"),
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
                "FileNameImage",
                new MemoryStream()),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The nugget with id DE540E54-DCF8-4717-AF50-332BCC1DFE2F is not found.");

        _nuggetRepository.Verify(
            e => e.GetById(Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F"), It.IsAny<CancellationToken>()),
            Times.Once);
        _userRepository.Verify(e => e.CheckIfIsAdmin(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
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

        _nuggetRepository.Verify(e => e.UpdateUrlImageAsync(
                It.IsAny<Nugget>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    
    [Fact]
    public async Task UpdateImageAsyncThrowNuggetImageIsEmptyException()
    {
        var act = async () => await _nuggetDomain.UpdateImageAsync(
            new UpdateNuggetImageCommand(
                Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6"),
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
                "newFileNameImage",
                new MemoryStream()),
            CancellationToken.None);

        await act.Should().ThrowAsync<NuggetImageIsEmptyException>()
            .WithMessage("The image cannot be empty.");

        _nuggetRepository.Verify(
            e => e.GetById(Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6"), It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepository.Verify(e => e.CheckIfIsAdmin(
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _fileStorage.Verify(f =>
                f.UploadFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MemoryStream>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);

        _fileStorage.Verify(f =>
                f.DeleteFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);

        _nuggetRepository.Verify(e => e.UpdateUrlImageAsync(
                It.IsAny<Nugget>(),
            It.IsAny<CancellationToken>()),
        Times.Never);
    }
    
    [Fact]
    public async Task UpdateImageAsyncUpdateNuggetWithNormalUser()
    {
        byte[] mockData = { 116, 101, 115, 116 };

        await _nuggetDomain.UpdateImageAsync(
            new UpdateNuggetImageCommand(
                Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6"),
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
                "newFileNameImage",
                new MemoryStream(mockData)),
            CancellationToken.None);

        _nuggetRepository.Verify(
            e => e.GetById(Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6"), It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepository.Verify(e => e.CheckIfIsAdmin(
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
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

        _nuggetRepository.Verify(e => e.UpdateUrlImageAsync(
                It.Is<Nugget>(n => n.UserId == Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675") &&
                                   n.Title == "Title" &&
                                   n.Content == "Content" &&
                                   n.UrlImage == "https://nuggets-images.host/2020-03-06T14:13:00Z-newFileNameImage" &&
                                   n.CreatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant() &&
                                   n.UpdatedAt == FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant()),
            It.IsAny<CancellationToken>()),
        Times.Once);
    }
    
    private void SetupNuggetRepositoryMethods()
    {
        _nuggetRepository.Setup(r => r.GetById(
                Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Nugget(
                Guid.Parse("3F7132FC-7F73-40BE-88A8-F94D8CC933D6"),
                "Title",
                "Content",
                "https://nuggets-images.host/2020-03-06T14:13:00Z-FileNameImage",
                Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"),
                FakeClock.FromUtc(2020, 3, 6, 14, 13, 0).GetCurrentInstant(),
                null));
        
        _nuggetRepository.Setup(r =>
                r.GetById(
                    Guid.Parse("DE540E54-DCF8-4717-AF50-332BCC1DFE2F"),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Nugget)null!);
        
        _userRepository.Setup(r =>
                r.CheckIfIsAdmin(Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC699"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _userRepository.Setup(r =>
                r.CheckIfIsAdmin(Guid.Parse("DAEAE728-BF12-4620-837A-81A6614CC675"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }
}