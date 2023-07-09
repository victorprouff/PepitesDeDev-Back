using System.Net;
using Core.Exceptions;

namespace Core.UserAggregate.Exceptions;

public class BadEmailFormatException : BaseException
{
    public BadEmailFormatException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}