using System.ComponentModel.DataAnnotations;

namespace Users.Dtos
{
    public record CreateUserDto
    {
        [Required]
        public string FirstName { get; init; }
        [Required]
        public string LastName { get; init; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; init; }
        [StringLength(30, ErrorMessage = "Must be between 8 and 30 characters", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; init; }
    }
}