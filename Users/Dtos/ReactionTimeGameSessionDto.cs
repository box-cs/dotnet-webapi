using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Users.Dtos
{
    public class ReactionTimeGameSessionDto
    {
        [Required]
        public string UserId { get; init; }
        [MinLength(1)]
        [Required]
        public List<long> ReactionTimes { get; init; }
    }
}