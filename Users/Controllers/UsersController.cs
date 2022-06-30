using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository repository;

        public UsersController(IUsersRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet] // GET /users
        public IEnumerable<UserDto> GetUsers()
        {
            var users = repository.GetUsers().Select(user => user.AsDto());
            return users;
        }

        [HttpGet("{id:guid}")] // Get /users/{id}
        public ActionResult<UserDto> GetUser(Guid id)
        {
            var user = repository.GetUser(id);
            return user is null ? NotFound() : user.AsDto();
        }

        [HttpPost] // POST /users
        public ActionResult<UserDto> CreateUser(CreateUserDto userDto)
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
            
            repository.CreateUser(user);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user.AsDto());
        }

        [HttpPut("{id:guid}")] // PUT /users
        public ActionResult UpdateUser(Guid id, UpdateUserDto userDto)
        {
            var existingUser = repository.GetUser(id);
            if (existingUser is null) return NotFound();

            User updatedUser = existingUser with
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Password = Hash.GeneratePassword(userDto.Password),
            };
            
            repository.UpdateUser(updatedUser);
            
            return NoContent();
            
        }

        [HttpDelete("{id:guid}")] // Delete /users/{id}
        public ActionResult DeleteUser(Guid id)
        {
            var user = repository.GetUser(id);
            if (user is null) return NotFound();
            repository.DeleteUser(id);

            return NoContent();
        }
    }
}