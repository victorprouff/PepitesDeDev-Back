namespace Core.Interfaces;

public interface IFileStorage
{
    public Task UploadFileAsync(string bucketName, string fileName, MemoryStream stream, CancellationToken cancellationToken);
    public Task DeleteFileAsync(string bucketName, string fileName, CancellationToken cancellationToken);
}