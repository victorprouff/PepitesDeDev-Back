using System.ComponentModel.DataAnnotations;
using Core.Specifications;

namespace Api.Models.Users;

public record CreateUserRequest(
    [Required]
    [RegularExpression(EmailSpecification.EmailRegexPattern)]string Email,
    [Required]string Username,
    [Required]
    [RegularExpression(PasswordSpecification.PasswordRegexPattern)]string Password);