using System;
using System.Collections.Generic;

namespace Users.Api.Entities
{
    public record ReactionTime
    {
        public Guid Id { get; set; }
        public string UserId { get; init; }
        public List<long> ReactionTimes { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}