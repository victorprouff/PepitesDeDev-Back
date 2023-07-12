using Amazon.S3;
using Amazon.S3.Model;
using Core.Interfaces;

namespace Infrastructure.Storages;

public class FileStorage : IFileStorage
{
    private readonly IAmazonS3 _client;

    public FileStorage(IAmazonS3 client)
    {
        _client = client;
    }
    
    public async Task<bool> UploadFileAsync(string bucketName, string fileName, MemoryStream stream)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = fileName,
            InputStream = stream,
            CannedACL  = S3CannedACL.PublicRead
        };
        
        var response = await _client.PutObjectAsync(request);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return true;
        }

        Console.WriteLine($"Could not upload {fileName} to {bucketName}.");
        return false;
    }
}