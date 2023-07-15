using System.Net;
using Core.Exceptions;

namespace Core.NuggetAggregate.Exceptions;

public class NotBeCreatedException : BaseException
{
    public NotBeCreatedException(string message, Exception exception)
        : base(message, exception, HttpStatusCode.InternalServerError)
    {
    }
    
    public NotBeCreatedException(string message)
        : base(message, HttpStatusCode.InternalServerError)
    {
    }
}