using System.Net;
using Core.Exceptions;

namespace Infrastructure.Storages.Exceptions;

public class FileCanNotUpdateException : BaseException
{
    public FileCanNotUpdateException(string message, Exception exception)
        : base(message, exception, HttpStatusCode.InternalServerError)
    {
    }
    
    public FileCanNotUpdateException(string message)
        : base(message, HttpStatusCode.InternalServerError)
    {
    }
}