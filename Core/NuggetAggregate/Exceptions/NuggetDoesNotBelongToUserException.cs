using System.Net;
using Core.Exceptions;

namespace Core.NuggetAggregate.Exceptions;

public class NuggetDoesNotBelongToUserException : BaseException
{
    public NuggetDoesNotBelongToUserException(string message, Exception exception)
        : base(message, exception, HttpStatusCode.BadRequest)
    {
    }
    
    public NuggetDoesNotBelongToUserException(string message)
        : base(message, HttpStatusCode.BadRequest)
    {
    }
}