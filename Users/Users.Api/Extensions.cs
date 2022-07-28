using Users.Api.Dtos;
using Users.Api.Entities;

namespace Users.Api
{
    public static class Extensions
    {
        public static UserDto AsDto(this User user)
        {
            return new UserDto{
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                CreatedDate = user.CreatedDate
            };
        }
        
        public static ReactionTimeGameSessionDto AsDto(this ReactionTime reactionTimeSession)
        {
            return new ReactionTimeGameSessionDto {
                UserId = reactionTimeSession.UserId,
                ReactionTimes = reactionTimeSession.ReactionTimes
            };
        }
    }
}