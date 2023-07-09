using System.Net;
using Core.Exceptions;

namespace Core.NuggetAggregate.Exceptions;

public class NuggetDoesNotBelongToUserException : BaseException
{
    public NuggetDoesNotBelongToUserException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}