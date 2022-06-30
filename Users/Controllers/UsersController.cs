﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Users.Dtos;
using Users.Entities;
using Users.Repositories;

namespace Users.Controllers
{
    // [ApiKeyAuth]
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

        /// <summary>
        /// Updates user fields that are passed in
        /// </summary>
        [HttpPut("{id:guid}")] // PUT /users 
        public ActionResult UpdateUser(Guid id, UpdateUserDto userDto)
        {
            var existingUser = repository.GetUser(id);
            if (existingUser is null) return NotFound();

            User updatedUser = existingUser with
            {
                FirstName = userDto.FirstName ?? existingUser.FirstName,
                LastName = userDto.LastName ?? existingUser.LastName ,
                Email = userDto.Email ?? existingUser.Email,
                Password = userDto.Password is not null ? Hash.GeneratePassword(userDto.Password) : existingUser.Password,
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

        [HttpPost("login")]
        public ActionResult Login(string email, string password)
        {
            var user = repository.GetUser(email);
            if (user is null) return NotFound();

            if (Hash.CompareHashes(password,user.Password))
                return Ok(
                    new
                    {
                     user.Id,
                     user.FirstName,
                     user.LastName,
                     user.Email,
                     user.CreatedDate
                    });
            return NotFound();
        }
    }
}