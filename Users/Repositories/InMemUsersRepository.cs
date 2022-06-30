using System;
using System.Collections.Generic;
using System.Linq;
using Users.Entities;

namespace Users.Repositories
{
    public class InMemUsersRepository : IUsersRepository
    {
        private readonly List<User> users = new()
        {
            new User { Id = Guid.NewGuid(), FirstName = "Jason", LastName = "Required", Email = "J@Gmail.com",  Password = Hash.GeneratePassword("123"), CreatedDate = DateTimeOffset.Now},
            new User { Id = Guid.NewGuid(), FirstName = "Stove", LastName = "Send", Email = "S@Gmail.com", Password = Hash.GeneratePassword("123"), CreatedDate = DateTimeOffset.Now},
            new User { Id = Guid.NewGuid(), FirstName = "Fox", LastName = "er", Email = "F@Gmail.com", Password = Hash.GeneratePassword("123"), CreatedDate = DateTimeOffset.Now}
        };

        public IEnumerable<User> GetUsers()
        {
            return users;
        }

        public void CreateUser(User user)
        {
            users.Add(user);
        }

        public void UpdateUser(User user)
        {
            var index = users.FindIndex(existingUser => existingUser.Id == user.Id);
            users[index] = user;
        }

        public void DeleteUser(Guid id)
        {
            var index = users.FindIndex(existingUser => existingUser.Id == id);
            users.RemoveAt(index);
        }

        public User GetUser(Guid id)
        {
            return users.Where(item => item.Id == id).SingleOrDefault();
        }
    }
}