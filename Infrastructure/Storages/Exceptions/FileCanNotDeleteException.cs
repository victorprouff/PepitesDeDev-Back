using System.Net;
using Core.Exceptions;

namespace Infrastructure.Storages.Exceptions;

public class FileCanNotDeleteException : BaseException
{
    public FileCanNotDeleteException(string message, Exception exception)
        : base(message, exception, HttpStatusCode.InternalServerError)
    {
    }
    
    public FileCanNotDeleteException(string message)
        : base(message, HttpStatusCode.InternalServerError)
    {
    }
}