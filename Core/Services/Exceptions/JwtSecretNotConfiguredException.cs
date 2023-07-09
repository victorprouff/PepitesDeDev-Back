using System.Net;
using Core.Exceptions;

namespace Core.Services.Exceptions;

public class JwtSecretNotConfiguredException : BaseException
{
    public JwtSecretNotConfiguredException(string message) : base(message, HttpStatusCode.BadRequest)
    {
    }
}