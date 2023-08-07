using System.Net;
using Core.Exceptions;

namespace Core.NuggetAggregate.Exceptions;

public class NuggetImageIsEmptyException : BaseException
{
    public NuggetImageIsEmptyException(string message, Exception exception)
        : base(message, exception, HttpStatusCode.BadRequest)
    {
    }
    
    public NuggetImageIsEmptyException(string message)
        : base(message, HttpStatusCode.BadRequest)
    {
    }
}