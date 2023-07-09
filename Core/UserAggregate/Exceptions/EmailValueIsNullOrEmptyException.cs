using System.Net;
using Core.Exceptions;

namespace Core.UserAggregate.Exceptions;

public class EmailValueIsNullOrEmptyException : BaseException
{
    public EmailValueIsNullOrEmptyException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}