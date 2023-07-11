namespace Core.Interfaces;

public interface IFileStorage
{
    public Task<bool> UploadFileAsync(string bucketName, string objectName, MemoryStream stream);
}