using System.Net;
using Core.Exceptions;

namespace Infrastructure.Storages.Exceptions;

public class FileCanNotUploadException : BaseException
{
    public FileCanNotUploadException(string message, Exception exception)
        : base(message, exception, HttpStatusCode.InternalServerError)
    {
    }
    
    public FileCanNotUploadException(string message)
        : base(message, HttpStatusCode.InternalServerError)
    {
    }
}