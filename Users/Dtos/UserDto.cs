using System;

namespace Users.Dtos
{
    public record UserDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}