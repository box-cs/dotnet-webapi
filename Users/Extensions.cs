using Users.Dtos;
using Users.Entities;

namespace Users
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
    }
}