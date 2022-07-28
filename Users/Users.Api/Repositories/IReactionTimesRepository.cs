using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Api.Entities;

namespace Users.Api.Repositories
{
    public interface IReactionTimesRepository
    {
        Task<ReactionTime> GetReactionTime(Guid userId);
        Task<IEnumerable<ReactionTime>> GetReactionTimesAsync();
        Task CreateReactionTime(ReactionTime reactionTime);
    }
}