using System.Globalization;
using System.Text.RegularExpressions;

namespace Core.UserAggregate;

public class Email
{
    public const string EmailRegexPattern = @"^[^@\s]+@[^@\s]+\.[a-zA-Z]+$";
    
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception("Value should not be empty or null");
        }
        
        var emailToLower = value.ToLower(CultureInfo.CurrentCulture);
        
        if (!Regex.IsMatch(emailToLower, EmailRegexPattern))
        {
            throw new Exception("The format of the email is not correct");
        }

        Value = value;
    }

    public string Value { get; }
}