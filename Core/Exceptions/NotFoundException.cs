using System.Net;

namespace Core.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message, HttpStatusCode.NotFound)
    {
    }
}