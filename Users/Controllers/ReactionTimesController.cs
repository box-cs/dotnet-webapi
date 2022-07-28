using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Users.Dtos;
using Users.Entities;
using Users.Filters;
using Users.Repositories;

namespace Users.Controllers
{
    [ApiKeyAuth]
    [ApiController]
    [Route("[controller]")]
    public class ReactionTimesController : ControllerBase
    {
        private readonly IReactionTimesRepository repository;

        public ReactionTimesController(IReactionTimesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet] // GET /statistics
        public async Task<IEnumerable<ReactionTimeGameSessionDto>> GetReactionTimesAsync()
        {
            var users = (await repository.GetReactionTimesAsync())
                .Select(session => session.AsDto());
            return users;
        }

        [HttpGet("{userId:guid}")] // Get /statistics/{id}
        public async Task<ActionResult<ReactionTimeGameSessionDto>> GetReactionTime(Guid userId)
        {
            var statistic = await repository.GetReactionTime(userId);
            return statistic is null ? NotFound() : statistic.AsDto();
        }

        [HttpPost] // POST /statistics
        public async Task<ActionResult<ReactionTimeGameSessionDto>> CreateReactionTime(ReactionTimeGameSessionDto reactionTime)
        {
            ReactionTime reactionTimeSession = new()
            {
                Id = new Guid(),
                UserId = reactionTime.UserId,
                ReactionTimes = reactionTime.ReactionTimes,
                CreatedDate = DateTimeOffset.Now
            };

            await repository.CreateReactionTime(reactionTimeSession);
            return CreatedAtAction("CreateReactionTime", new { id = reactionTimeSession.UserId }, reactionTimeSession);
        }
    }
}