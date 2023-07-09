using System.Net;
using Core.Exceptions;

namespace Core.UserAggregate.Exceptions;

public class BadPasswordException : BaseException
{
    public BadPasswordException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}