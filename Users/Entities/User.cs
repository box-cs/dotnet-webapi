using System;

namespace Users.Entities
{
    // Record Type:
    // Immutability
    // With-Expression Support
    // Value-Based Equality Support
    public record User
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}