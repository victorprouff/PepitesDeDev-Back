namespace Core.Specifications;

public class PasswordSpecification
{
    public const string PasswordRegexPattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";
}