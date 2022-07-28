using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Users.Api.Dtos;
using Users.Api.Entities;
using Users.Api.Filters;
using Users.Api.Repositories;
using Users.Api.Settings;

namespace Users.Api.Controllers
{
    [ApiKeyAuth]
    [ApiController]
    [Route("[controller]")]
    public class UserLoginController : ControllerBase
    {
        private readonly IUsersRepository repository;

        public UserLoginController(IUsersRepository repository)
        {
            this.repository = repository;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(LoginUserDto userDto)
        {
            var user = await repository.GetUserAsync(userDto.Email);
            if (user is null) return NotFound();

            if (Hash.CompareHashes(userDto.Password, user.Password))
                return Ok(
                    new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.CreatedDate,
                        token = Generate(user),
                    });
            return NotFound("User Not Found");
        }

        [AllowAnonymous]
        [HttpPost("update-token")]
        public async Task<ActionResult> UpdateToken(LoginUserDto userDto)
        {
            var user = await repository.GetUserAsync(userDto.Email);
            if (user is null) return NotFound();

            return Ok(
                new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.CreatedDate,
                    token = Generate(user),
                });
        }

        [HttpGet("current")]
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

        [HttpGet("refresh-token")]
        [Authorize]
        public async Task<ActionResult> RefreshToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            if (HttpContext.User.Identity is not ClaimsIdentity identity) return NotFound();
            var userClaims = identity.Claims;

            var claims = userClaims as Claim[] ?? userClaims.ToArray();
            User currUser =
                await repository.GetUserAsync(claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);

            if (currUser is null) return NoContent();
            var newToken = new JwtSecurityToken(
                Jwt.Issuer,
                Jwt.Audience,
                claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: credentials);

            string refreshedToken = new JwtSecurityTokenHandler().WriteToken(newToken);
            return Ok(refreshedToken);
        }

        private string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                Jwt.Issuer,
                Jwt.Audience,
                claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}