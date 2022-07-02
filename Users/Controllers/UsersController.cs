using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors; // TODO Remember to remove cors and all EnableCors attributes
using Microsoft.AspNetCore.Mvc;
using Users.Dtos;
using Users.Entities;
using Users.Repositories;

namespace Users.Controllers
{
    // [ApiKeyAuth]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository repository;

        public UsersController(IUsersRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet] // GET /users
        [EnableCors("AllowOrigin")]
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = (await repository.GetUsersAsync())
                .Select(user => user.AsDto());
            return users;
        }

        [HttpGet("{id:guid}")] // Get /users/{id}
        [EnableCors("AllowOrigin")]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid id)
        {
            var user = await repository.GetUserAsync(id);
            return user is null ? NotFound() : user.AsDto();
        }

        [HttpGet("/Current")]
        [Authorize]
        public ActionResult GetCurrentUser()
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity) return NotFound();
            var userClaims = identity.Claims;
            
            return Ok(
                new
                {
                    Id = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                    FirstName = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    LastName = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
                    Email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                });
        }

        [HttpPost] // POST /users
        [EnableCors("AllowOrigin")]
        public async Task<ActionResult<UserDto>> CreateUserAsync(CreateUserDto userDto)
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Password = Hash.GeneratePassword(userDto.Password),
                Email = userDto.Email,
                CreatedDate = DateTimeOffset.Now
            };

            await repository.CreateUserAsync(user);
            return CreatedAtAction("GetUser", new { id = user.Id }, user.AsDto());
        }

        [HttpPut("{id:guid}")] // PUT /users 
        [EnableCors("AllowOrigin")]
        public async Task<ActionResult> UpdateUserAsync(Guid id, UpdateUserDto userDto)
        {
            var existingUser = await repository.GetUserAsync(id);
            if (existingUser is null) return NotFound();

            User updatedUser = existingUser with
            {
                FirstName = userDto.FirstName ?? existingUser.FirstName,
                LastName = userDto.LastName ?? existingUser.LastName,
                Email = userDto.Email ?? existingUser.Email,
                Password = userDto.Password is not null
                    ? Hash.GeneratePassword(userDto.Password)
                    : existingUser.Password,
            };

            await repository.UpdateUserAsync(updatedUser);

            return NoContent();
        }

        [HttpDelete("{id:guid}")] // Delete /users/{id}
        [EnableCors("AllowOrigin")]
        public async Task<ActionResult> DeleteUserAsync(Guid id)
        {
            var user = repository.GetUserAsync(id);
            if (user is null) return NotFound();
            await repository.DeleteUserAsync(id);

            return NoContent();
        }
    }
}