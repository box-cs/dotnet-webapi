using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Users.Api;
using Users.Api.Controllers;
using Users.Api.Dtos;
using Users.Api.Entities;
using Users.Api.Repositories;
using Xunit;

namespace Users.UnitTests;

public class UsersControllerTests
{
    private readonly Mock<IUsersRepository> respositoryStub = new();
    private readonly Mock<ILogger<UsersController>> loggerStub = new();

    [Fact]
    public async Task GetUserAsync_InexistantUser_ReturnsNotFound()
    {
        // Arrange
        respositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
            .ReturnsAsync(null as User);

        var controller = new UsersController(respositoryStub.Object, loggerStub.Object);
        // Act
        var result = await controller.GetUserAsync(Guid.NewGuid());
        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetUserAsync_ExistingUser_ReturnsExpectedUser()
    {
        var expectedUser = CreateRandomUser();
        respositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedUser);

        var controller = new UsersController(respositoryStub.Object, loggerStub.Object);
        var result = await controller.GetUserAsync(Guid.NewGuid());

        result.Value.Should().BeEquivalentTo(expectedUser, options => options.ComparingByMembers<User>());
    }

    [Fact]
    public async Task GetUsersAsync_ExistingUsers_ReturnsAllUsers()
    {
        var expectedUsers = new[] { CreateRandomUser(), CreateRandomUser(), CreateRandomUser() };
        respositoryStub.Setup(repo => repo.GetUsersAsync())
            .ReturnsAsync(expectedUsers);

        var controller = new UsersController(respositoryStub.Object, loggerStub.Object);
        var users = await controller.GetUsersAsync();
        users.Should().BeEquivalentTo(expectedUsers, options => options.ComparingByMembers<User>());
    }

    [Fact]
    public async Task CreateUserAsync_WithUserToCreate_ReturnsCreatedUser()
    {
        var userToCreate = new CreateUserDto()
        {
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString(),
            Email = "test@email.com",
            Password = Guid.NewGuid().ToString(),
        };

        var controller = new UsersController(respositoryStub.Object, loggerStub.Object);

        var result = await controller.CreateUserAsync(userToCreate);
        var createdUser = (result.Result as CreatedAtActionResult)?.Value as UserDto;

        Assert.Equal(userToCreate.FirstName, createdUser?.FirstName);
        Assert.Equal(userToCreate.LastName, createdUser?.LastName);
        Assert.Equal(userToCreate.Email, createdUser?.Email);
        Assert.Equal(Hash.CompareHashes(userToCreate.Password, createdUser?.Password), true);
        createdUser?.Id.Should().NotBeEmpty();
        createdUser?.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 0, 0, 0, 1000));
    }

    [Fact]
    public async Task UpdateUserASync_WithExistingUser_ReturnsNoContent()
    {
        User existingUser = CreateRandomUser(hashedPassword: true);
        respositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingUser);

        var userId = existingUser.Id;
        var userToUpdate = new UpdateUserDto()
        {
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString(),
            Email = "anotherTest@email.com",
            Password = Guid.NewGuid().ToString(),
        };
        var controller = new UsersController(respositoryStub.Object, loggerStub.Object);
        var result = await controller.UpdateUserAsync(userId, userToUpdate);
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteUserASync_WithExistingUser_ReturnsNoContent()
    {
        User existingUser = CreateRandomUser();
        respositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingUser);

        var controller = new UsersController(respositoryStub.Object, loggerStub.Object);
        var result = await controller.DeleteUserAsync(existingUser.Id);
        result.Should().BeOfType<NoContentResult>();
    }

    private User CreateRandomUser(bool hashedPassword = false)
    {
        string Password = Guid.NewGuid().ToString();
        return new()
        {
            Id = Guid.NewGuid(),
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString(),
            Email = "test@email.com",
            Password = hashedPassword ? Hash.GeneratePassword(Password) : Password,
        };
    }
}